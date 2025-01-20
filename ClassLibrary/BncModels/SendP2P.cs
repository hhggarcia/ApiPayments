using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class SendP2P
    {
        [Range(0, 999999999999.99)]
        public decimal Amount { get; set; }
        [Range(0, int.MaxValue)]
        public int BeneficiaryBankCode { get; set; }
        [Phone]
        public string BeneficiaryCellPhone { get; set; } = string.Empty;
        [EmailAddress]
        public string? BeneficiaryEmail { get; set; } = null!;
        [RegularExpression(@"^[A-Z]{1}\d{8,9}$", ErrorMessage = "Formato de ID inválido.")]
        public string BeneficiaryID { get; set; } = string.Empty;
        [StringLength(100)]
        public string BeneficiaryName { get; set; } = string.Empty;
       // [StringLength(6)]
        public string? BranchID { get; set; }
        //[RegularExpression(@"^[A-Z]{1}\d{8,9}$", ErrorMessage = "Formato de ID inválido.")]
        public string? ChildClientID { get; set; }
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
        [StringLength(20)]
        public string? OperationRef { get; set; } = null!;
    }

    public class SendP2PResponse
    {
        public string Reference { get; set; } = string.Empty;
        public string AuthorizationCode { get; set; } = string.Empty;
        public bool SwSwAlreadySent { get; set; }
    }
}
