using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace StaffManagement.Models
{
    public class DropDownViewModel : BaseViewModel
    {
        public DropDownViewModel()
        {
            Items = new Dictionary<string, string>();

        }
        public Dictionary<string, string> Items { get; set; }
        public string SelectedValue { get; set; }
        public string DefaultValue { get; set; }

        public DropDownViewModel Add(string key, string value)
        {
            Items.Add(key, value);
            return this;

        }
        public DropDownViewModel Add(SelectList selectList)
        {
            Items = (Dictionary<string, string>)selectList.Items;
            return this;

        }

    }
}
