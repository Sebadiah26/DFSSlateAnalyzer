using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StaffManagement.Models
{

    /// <summary>
    /// Account object model with account properties and  related employee, user, jobrecord objects
    /// </summary>
    [BindProperties(SupportsGet = true)]
    public class TitleAdminModel : BaseViewModel
    {

        public TitleAdminModel()
        {
            AllTitles = new List<TitleModel>();
            TitleGroups = new List<TitleGroupModel>();
        }

        public int SelectedTitleId { get; set; }

        public string SelectedTitleText { get; set; }


        public int SelectedTitleGroupId { get; set; }



        public virtual List<TitleModel> AllTitles { get; set; }


        [Display(Name = "Title Group")]
        public virtual List<TitleGroupModel> TitleGroups { get; set; }

        private SelectList _titlegroupselectlist = null;
        public SelectList TitleGroupsSelectList

        {
            get
            {
                if (_titlegroupselectlist == null)
                {
                    if (TitleGroups.Any() == true)
                    {

                        _titlegroupselectlist = new SelectList(TitleGroups.ToDictionary(x => x.TitleGroupId, x => x.Description), "Key", "Value");
                    }
                }

                return _titlegroupselectlist;
            }
        }














    }
}
