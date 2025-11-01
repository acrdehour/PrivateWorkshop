using System.ComponentModel.DataAnnotations;

namespace PrivateWorkshop.Models.Enums
{
    public enum BookingStatus
    {
        [Display(Name = "รอดำเนินการ")]
        Pending = 1,

        [Display(Name = "อนุมัติแล้ว")]
        Approved = 2,

        [Display(Name = "ถูกปฏิเสธ")]
        Rejected = 3,

        [Display(Name = "ยกเลิก")]
        Cancelled = 4
    }
}
