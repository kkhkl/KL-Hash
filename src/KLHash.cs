using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace KLHash;

public partial class MainForm : Form
{
	private static readonly long ProgressReportIntervalTicks = TimeSpan.FromMilliseconds(250).Ticks;
	private static readonly long UiRefreshIntervalTicks = TimeSpan.FromMilliseconds(100).Ticks;
	private const int BufferSizeMb = 4;
	private const int BufferSizeBytes = BufferSizeMb * 1024 * 1024;
	private CancellationTokenSource? _cts;
	private string[]? _currentFilePaths;
	private string[]? _rawHashValuesCache;
	private string? _successStatusCache;
	private readonly Font _fontRegular = new("Consolas", 12F, FontStyle.Regular);
	private readonly Font _fontBold = new("Consolas", 12F, FontStyle.Bold);

	public MainForm()
	{
		InitializeComponent();
		FormClosing += (_, _) =>
		{
			_cts?.Cancel();
			_cts?.Dispose();
		};
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_fontRegular.Dispose();
			_fontBold.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		ResetUiState(clearCache: true);
		ActiveControl = btnBrowse;
	}

	private void ResetUiState(bool clearCache)
	{
		txtDisplay.ReadOnly = false;
		txtDisplay.Text = "拖拽文件到此界面进立即进行计算";
		txtDisplay.ForeColor = SystemColors.GrayText;
		txtDisplay.Font = _fontRegular;
		txtDisplay.ReadOnly = true;
		txtDisplay.BackColor = Color.White;
		if (clearCache)
		{
			_rawHashValuesCache = null;
			_currentFilePaths = null;
			_successStatusCache = null;
			btnCopy.Enabled = false;
		}
	}

	private void SetUiComputingState(bool isComputing)
	{
		btnBrowse.Enabled = !isComputing;
		btnCancel.Visible = isComputing;
		btnCancel.Enabled = isComputing;
		progressBar.Visible = isComputing;
		if (isComputing)
		{
			btnCopy.Enabled = false;
			progressBar.Value = 0;
		}
	}

	private void UpdateDisplayResult()
	{
		if (_currentFilePaths is not { Length: > 0 } || _rawHashValuesCache is not { Length: > 0 } || _currentFilePaths.Length != _rawHashValuesCache.Length)
		return;
		var sb = new StringBuilder();
		bool isUpper = chkUpperCase.Checked;
		for (int i = 0; i < _currentFilePaths.Length; i++)
		{
			string? rawHash = _rawHashValuesCache[i];
			if (rawHash is null) continue;
			string fileName = Path.GetFileName(_currentFilePaths[i]);
			sb.AppendLine(fileName);
			if (rawHash.StartsWith('['))
			sb.AppendLine(rawHash);
			else
			sb.AppendLine($"SHA-256: {(isUpper ? rawHash.ToUpperInvariant() : rawHash.ToLowerInvariant())}");
			sb.AppendLine();
		}
		if (sb.Length == 0) return;
		txtDisplay.ReadOnly = false;
		txtDisplay.Font = _fontBold;
		txtDisplay.ForeColor = Color.FromArgb(0, 150, 136);
		txtDisplay.Text = sb.ToString().TrimEnd();
		txtDisplay.ReadOnly = true;
		txtDisplay.BackColor = Color.White;
		btnCopy.Enabled = true;
	}

	private void SetError(string userMessage, string detailMessage)
	{
		txtDisplay.ReadOnly = false;
		txtDisplay.Font = _fontBold;
		txtDisplay.ForeColor = Color.Red;
		txtDisplay.Text = userMessage;
		txtDisplay.ReadOnly = true;
		SetStatus(detailMessage, isError: true);
		btnCopy.Enabled = false;
	}

	private void SetStatus(string text, bool isError = false)
	{
		lblStatus.Text = text;
		lblStatus.ForeColor = isError ? Color.Red : Color.DimGray;
	}

	private void OnDragEnter(object? sender, DragEventArgs e)
	{
		if (!btnBrowse.Enabled)
		{
			e.Effect = DragDropEffects.None;
			return;
		}
		e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) is true
		? DragDropEffects.Copy
		: DragDropEffects.None;
	}

	private async void OnDragDrop(object? sender, DragEventArgs e)
	{
		if (!btnBrowse.Enabled || e.Data == null) return;
		if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
		e.Data.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } files)
		{
			await StartComputeAsync(files);
		}
	}

	private async void OnBrowseClick(object? sender, EventArgs e)
	{
		if (!btnBrowse.Enabled) return;
		using var ofd = new OpenFileDialog
		{
			Title = "选择要计算哈希的文件",
			Filter = "所有文件 (*.*)|*.*",
			Multiselect = true
		};
		if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Length > 0)
		{
			await StartComputeAsync(ofd.FileNames);
		}
	}

	private void OnCancelClick(object? sender, EventArgs e)
	{
		_cts?.Cancel();
		btnCancel.Enabled = false;
		SetStatus("正在取消...");
	}

	private void OnCopyClick(object? sender, EventArgs e)
	{
		if (_rawHashValuesCache is not { Length: > 0 } || string.IsNullOrEmpty(txtDisplay.Text))
		return;
		Clipboard.SetText(txtDisplay.Text);
		SetStatus(!string.IsNullOrEmpty(_successStatusCache)
		? $"{_successStatusCache}，Hash已复制"
		: "Hash已复制到剪贴板");
	}

	private void OnCaseToggle(object? sender, EventArgs e) => UpdateDisplayResult();

	private async Task StartComputeAsync(string[] filePaths)
	{
		SetUiComputingState(isComputing: true);
		_currentFilePaths = filePaths;
		_rawHashValuesCache = new string[filePaths.Length];
		ResetUiState(clearCache: false);
		_cts?.Cancel();
		_cts = new CancellationTokenSource();
		var currentCts = _cts;
		var token = currentCts.Token;
		var stopwatch = Stopwatch.StartNew();
		int totalFiles = filePaths.Length;
		try
		{
			var progress = new Progress<int>(p => progressBar.Value = Math.Clamp(p, 0, 100));
			int errorCount = 0;
			long lastUiUpdateTicks = DateTime.UtcNow.Ticks;
			await Task.Run(() =>
			{
				for (int i = 0; i < totalFiles; i++)
				{
					token.ThrowIfCancellationRequested();
					string path = filePaths[i];
					string name = Path.GetFileName(path);
					Invoke(() => SetStatus($"正在计算 ({i + 1}/{totalFiles}): {name}"));
					try
					{
						_rawHashValuesCache[i] = ComputeHashInternalSync(path, progress, token);
					}
					catch (OperationCanceledException)
					{
						throw;
					}
					catch (FileNotFoundException)
					{
						_rawHashValuesCache[i] = "[错误: 文件不存在]";
						errorCount++;
					}
					catch (UnauthorizedAccessException)
					{
						_rawHashValuesCache[i] = "[错误: 无读取权限]";
						errorCount++;
					}
					catch (IOException ex) when ((ex.HResult & 0xFFFF) == 32)
					{
						_rawHashValuesCache[i] = "[错误: 文件被占用]";
						errorCount++;
					}
					catch (Exception ex)
					{
						_rawHashValuesCache[i] = $"[错误: {ex.Message}]";
						errorCount++;
					}
					long currentTicks = DateTime.UtcNow.Ticks;
					if (i == totalFiles - 1 || (currentTicks - lastUiUpdateTicks) >= UiRefreshIntervalTicks)
					{
						lastUiUpdateTicks = currentTicks;
						Invoke(UpdateDisplayResult);
					}
				}
			}, token);
			stopwatch.Stop();
			TimeSpan ts = stopwatch.Elapsed;
			string timeString = ts.TotalHours >= 1
			? $"{Math.Floor(ts.TotalHours)}小时{ts:mm'分'ss'.'ff'秒'}"
			: ts.TotalMinutes >= 1
			? $"{ts:mm'分'ss'.'ff'秒'}"
			: $"{ts:ss'.'ff'秒'}";
			if (errorCount > 0)
			{
				_successStatusCache = $"计算完成！（{totalFiles - errorCount} 成功 / {errorCount} 失败，总用时: {timeString}）";
				SetStatus(_successStatusCache, isError: true);
			}
			else
			{
				_successStatusCache = $"计算完成！共 {totalFiles} 个文件 (总用时: {timeString})";
				SetStatus(_successStatusCache);
			}
		}
		catch (OperationCanceledException)
		{
			ResetUiState(clearCache: true);
			SetStatus("已取消计算", isError: true);
		}
		finally
		{
			currentCts.Dispose();
			if (_cts == currentCts)
			{
				_cts = null;
			}
			SetUiComputingState(isComputing: false);
		}
	}

	private static string ComputeHashInternalSync(string filePath, IProgress<int> progress, CancellationToken ct)
	{
		byte[] buffer = new byte[BufferSizeBytes];
		using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSizeBytes, useAsync: false);
		long totalBytes = fs.Length;
		long totalRead = 0;
		int lastPercent = -1;
		int bytesRead;
		long lastTicks = DateTime.UtcNow.Ticks;
		using var hashAlgorithm = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		var bufferSpan = buffer.AsSpan();
		while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
		{
			ct.ThrowIfCancellationRequested();
			totalRead += bytesRead;
			int percent = totalBytes > 0 ? (int)(totalRead * 100 / totalBytes) : 0;
			long currentTicks = DateTime.UtcNow.Ticks;
			if (percent != lastPercent && (currentTicks - lastTicks >= ProgressReportIntervalTicks || percent == 100))
			{
				progress.Report(percent);
				lastPercent = percent;
				lastTicks = currentTicks;
			}
			hashAlgorithm.AppendData(bufferSpan[..bytesRead]);
		}
		if (lastPercent < 100)
		progress.Report(100);
		return Convert.ToHexString(hashAlgorithm.GetHashAndReset());
	}
}
