using System.ComponentModel.DataAnnotations;

namespace SmartPulseTask.Web.Models;

public class TransactionAnalysisViewModel
{
    [Required(ErrorMessage = "Başlangıç tarihi gereklidir")]
    [Display(Name = "Başlangıç Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Bitiş tarihi gereklidir")]
    [Display(Name = "Bitiş Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    public List<ContractAnalysisResultViewModel> Results { get; set; } = new();

    public bool HasResults => Results.Any();

    public string? ErrorMessage { get; set; }

    public bool IsLoading { get; set; }
}
