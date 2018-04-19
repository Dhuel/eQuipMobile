using System;

namespace eQuipMobile
{

    public class TransferClassData
    {
        public string TransactionName { get; set; }
        public string ToSite { get; set; }
        public string FromSite { get; set; }
        public string ToLocation { get; set; }
        public string FromLocation { get; set; }
        public string SiteNameTo { get; set; }
        public string SiteNameFrom { get; set; }
        public string LocationNameTo { get; set; }
        public string LocationNameFrom { get; set; }
        public string ToPeople { get; set; }
        public string FromPeople { get; set; }
        public string ToSublocation { get; set; }
        public string FromSublocation { get; set; }
        public int Quantitymoved { get; set; }
        public int Quantityprev { get; set; }
        public DateTime DateMoved { get; set; }
        public string AssetIdInternal { get; set; }
        public string AssetName { get; set; }
    }
        public class TransferClass
    {
        public string TransactionName { get; set; }
        public string ToSite { get; set; }
        public string ToLocation { get; set; }
        public string ToPeople { get; set; }
        public string ToSublocation { get; set; }
        public int Quantitymoved { get; set; }
        public DateTime DateMoved { get; set; }
        public string AssetIdInternal { get; set; }
        public TransferClass(TransferDbTable TransferDbData)
        {
            TransactionName = TransferDbData.TransactionName;
            ToSite = TransferDbData.ToSite;
            ToLocation = TransferDbData.ToLocation;
            ToPeople = TransferDbData.ToPeople;
            ToSublocation = TransferDbData.ToSublocation;
            Quantitymoved = TransferDbData.Quantitymoved;
            DateMoved = Convert.ToDateTime(TransferDbData.DateMoved);
            AssetIdInternal = TransferDbData.AssetIdInternal;
        }
        public TransferClass() { }
    }

    public class AssetTransferFormatClass
    {
        public string UserID { get; set; }
        public string TransactionName { get; set; }
        public string ToSite { get; set; }
        public string ToLocation { get; set; }
        public string ToPeople { get; set; }
        public string ToSublocation { get; set; }
        public int QtyMove { get; set; }
        public DateTime DateMoved { get; set; }
        public string AssetIDInternal { get; set; }
        public void SetUsingTransfer(TransferClass TransferData)
        {
            TransactionName = TransferData.TransactionName;
            ToSite = TransferData.ToSite;
            ToLocation = TransferData.ToLocation;
            ToPeople = TransferData.ToPeople;
            ToSublocation = TransferData.ToSublocation;
            QtyMove = TransferData.Quantitymoved;
            DateMoved = TransferData.DateMoved;
            AssetIDInternal = TransferData.AssetIdInternal;
        }
    }
}
