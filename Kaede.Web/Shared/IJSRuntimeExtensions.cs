using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Web.Shared {
    public static class IJSRuntimeExtensions {
        public static async ValueTask Alert(this IJSRuntime iJSRuntime, string text) {
            await iJSRuntime.InvokeVoidAsync("alert", text);
        }

        public static async ValueTask CopyToClipBoard(this IJSRuntime iJSRuntime, string text) {
            await iJSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }

        public static async ValueTask<bool> Confirm(this IJSRuntime iJSRuntime, string text) {
            return await iJSRuntime.InvokeAsync<bool>("confirm", text);
        }

        public static async ValueTask ConsoleLog(this IJSRuntime iJSRuntime, string text) {
            await iJSRuntime.InvokeVoidAsync("console.log", text);
        }
    }
}
