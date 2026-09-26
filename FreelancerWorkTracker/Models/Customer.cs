using System.ComponentModel.DataAnnotations;

namespace FreelancerWorkTracker.Models
{
    public class Customer
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Müşteri Adı")]
        public string Name { get; set; } = string.Empty;


        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [StringLength(150, ErrorMessage = "E-posta en fazla 150 karakter olabilir.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;


        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [StringLength(30, ErrorMessage = "Telefon numarası en fazla 30 karakter olabilir.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;


        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        [Display(Name = "Notlar")]
        public string Notes { get; set; } = string.Empty;


        public List<Project> Projects { get; set; } = new();


        public string UserId { get; set; } = string.Empty;


        public ApplicationUser? User { get; set; }
    }
}