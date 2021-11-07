using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class TransferRequest
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; } = 2;
        public int TransferStatusId { get; set; } = 2;
        public int UserFrom { get; set; }
        public int UserTo { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferReceipt
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; } = 2;
        public int TransferStatusId { get; set; } = 2;
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
    }
}
