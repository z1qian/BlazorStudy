using System.Globalization;
using System.Net.Http.Json;
using AntDesign.Extensions.Localization;
using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components;

namespace MyAntDesignApp.Layouts
{
    public partial class BasicLayout : LayoutComponentBase, IDisposable
    {
        private MenuDataItem[] _menuData;

        [Inject] private AntDesign.ReuseTabsService TabService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _menuData = new[] {
                new MenuDataItem
                {
                    Path = "/",
                    Name = "welcome",
                    Key = "welcome",
                    Icon = "smile",
                },
                  new MenuDataItem
                {
                    Path = "/login",
                    Name = "登录",
                    Key = "login",
                    Icon = "login",
                },
                  new MenuDataItem
                {
                    Path = "/prerendered-counter-1",
                    Name = "计算器",
                    Key = "counter",
                    Icon = "plus",
                }
            };
        }

        void Reload()
        {
            TabService.ReloadPage();
        }

        public void Dispose()
        {

        }

    }
}
