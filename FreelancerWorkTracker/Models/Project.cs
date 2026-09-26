using System.ComponentModel.DataAnnotations;

namespace FreelancerWorkTracker.Models
{
    public class Project : IValidatableObject
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Proje adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Proje adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Proje Adı")]
        public string Name { get; set; } = string.Empty;


        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;


        [Range(0, 999999999, ErrorMessage = "Proje ücreti negatif olamaz.")]
        [Display(Name = "Proje Ücreti")]
        public decimal Price { get; set; }


        [Range(0, 999999999, ErrorMessage = "Ödenen tutar negatif olamaz.")]
        [Display(Name = "Ödenen Tutar")]
        public decimal PaidAmount { get; set; }


        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }


        [Required(ErrorMessage = "Teslim tarihi zorunludur.")]
        [Display(Name = "Teslim Tarihi")]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }


        [Display(Name = "Durum")]
        public ProjectStatus Status { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir müşteri seçin.")]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }


        public Customer? Customer { get; set; }


        public string UserId { get; set; } = string.Empty;


        public ApplicationUser? User { get; set; }


        // Birden fazla alanı karşılaştırmamız gereken kontroller
        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (Deadline.Date < StartDate.Date)
            {
                yield return new ValidationResult(
                    "Teslim tarihi başlangıç tarihinden önce olamaz.",
                    new[] { nameof(Deadline) });
            }

            if (PaidAmount > Price)
            {
                yield return new ValidationResult(
                    "Ödenen tutar proje ücretinden fazla olamaz.",
                    new[] { nameof(PaidAmount) });
            }
        }
    }
}