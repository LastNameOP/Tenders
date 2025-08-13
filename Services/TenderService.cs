using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Testovoe.Models;


public class TenderService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TenderService> _logger;
    private readonly String? _tendersFilePath;

    public TenderService(ILogger<TenderService> logger , IConfiguration config)
    {
        _config = config;
        _logger = logger;
        _tendersFilePath = _config["TendersFile:Path"];
    }

    public async Task<List<TenderJson>> GetAllAsync(CancellationToken ct = default)
    {
        var path = _tendersFilePath;
        if (!File.Exists(path))
        {
            GetEmptyTenderJson("Исходный файл не найден");
        }
        if (string.IsNullOrEmpty(path))
        {
            GetEmptyTenderJson("Исходный файл повреждён или пуст");
        }


        using FileStream fs = File.OpenRead(path);

        IWorkbook workbook = Path.GetExtension(path).Equals(".xls") ? new HSSFWorkbook(fs) : new XSSFWorkbook(fs);

        var sheet = workbook.GetSheetAt(0);
        if (sheet == null)
            return GetEmptyTenderJson("Файл пуст");


        Dictionary<string, int> tableHeadersIndexes = GetTableHeaders(sheet);

        var list = new List<TenderJson>();
        for (int r = 1; r <= sheet.LastRowNum; r++)
        {
            var row = sheet.GetRow(r);
            if (row == null) continue;

            string getString(string headerName)
            {
                if (!tableHeadersIndexes.TryGetValue(headerName, out int ci)) return string.Empty;
                var cell = row.GetCell(ci);
                return cell?.ToString()?.Trim() ?? string.Empty;
            }

            DateTime? parseDate(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s, out var dt)) return dt;
                return null;
            }

            var title = getString("Название тендера");
            var start = getString("Дата начала");
            var end = getString("Дата окончания");
            var url = getString("URL тендерной площадки");

            bool isCellDate(IRow rrow, string headerName)
            {
                if (!tableHeadersIndexes.TryGetValue(headerName, out var ci)) return false;
                var cell = rrow.GetCell(ci);
                if (cell == null) return false;
                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                {
                    return true;
                }
                return false;
            }

            var sdt = isCellDate(row, "Дата начала") ? parseDate(start) : null;
            var edt = isCellDate(row, "Дата окончания") ? parseDate(end) : null;

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(url) && sdt == null && edt == null)
                continue;

            list.Add(new TenderJson
            {
                Title = title,
                StartDate = sdt,
                EndDate = edt,
                Url = url
            });
        }

        return await Task.FromResult(list);
    }

    private static Dictionary<string, int> GetTableHeaders(ISheet sheet)
    {
        var header = sheet.GetRow(0);
        var colIndex = new Dictionary<string, int>();
        for (int c = 0; c <= header.LastCellNum; c++)
        {
            var cell = header.GetCell(c);
            if (cell == null) continue;
            var value = cell.ToString().Trim();
            if (string.IsNullOrEmpty(value)) continue;
            colIndex[value] = c;
        }

        return colIndex;
    }

    private List<TenderJson> GetEmptyTenderJson(string message)
    {
        _logger.LogWarning($"{message}: {_tendersFilePath}");
        return [];
    }
}
