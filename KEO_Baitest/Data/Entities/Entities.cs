using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public abstract class Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeleteDate { get; set; }

        public string? DeleteBy { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public string? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? UpdateBy { get; set; }
    }
}
