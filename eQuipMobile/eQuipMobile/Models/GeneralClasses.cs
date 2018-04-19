using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace eQuipMobile
{
    public class GUID
    {
        public static string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
    public class WorkflowClass
    {
        public string TotalAssets { get; set; }
        public string AuditedAssets { get; set; }
        public string ReconMoved { get; set; }
        public string ReconQuantity { get; set; }
        public string ReconDepartment { get; set; }
        public string ReconCustodian { get; set; }
        public string ReconAdded { get; set; }
        public string NotAudited { get; set; }
        public WorkflowClass() { }
    }

    public class AppImages
    {
        public static Image ConvertTocontentImage(Stream image)
        {
            Image test_Image = new Image
            {
                Aspect = Aspect.AspectFit,
                Source = ImageSource.FromStream(() => image)
            };
            return test_Image;
        }
        public static string StreamToBase64String(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        public static Stream Base64toStream(string input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] b = Convert.FromBase64String(input);
                return new MemoryStream(b);
            }
        }
    }
    public class EntryLengthValidatorBehavior : Behavior<Entry>
    {
        public int MaxLength { get; set; }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            // if Entry text is longer then valid length
            if (entry.Text.Length > this.MaxLength)
            {
                string entryText = entry.Text;

                entryText = entryText.Remove(entryText.Length - 1); // remove last char

                entry.Text = entryText;
            }
        }
    }
}
