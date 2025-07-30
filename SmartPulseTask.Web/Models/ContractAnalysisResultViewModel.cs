using System.ComponentModel.DataAnnotations;

namespace SmartPulseTask.Web.Models;

public class ContractAnalysisResultViewModel
{
    [Display(Name = "Kontrat Adı")]
    public string ContractName { get; set; } = string.Empty;

    [Display(Name = "Tarih")]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime ContractDateTime { get; set; }

    [Display(Name = "Toplam İşlem Tutarı")]
    [DisplayFormat(DataFormatString = "{0:N2} ₺", ApplyFormatInEditMode = false)]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Toplam İşlem Miktarı")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
    public decimal TotalQuantity { get; set; }

    [Display(Name = "Ağırlıklı Ortalama Fiyat")]
    [DisplayFormat(DataFormatString = "{0:N2} ₺", ApplyFormatInEditMode = false)]
    public decimal WeightedAveragePrice { get; set; }

    [Display(Name = "İşlem Sayısı")]
    public int TransactionCount { get; set; }
}
