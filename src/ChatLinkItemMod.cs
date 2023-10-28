using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace ChatLinkItem;

public class ChatLinkItemMod : ModSystem {
    [SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to \'GeneratedRegexAttribute\'.")]
    private static readonly Regex ITEM_LINK = new(@"(\[item\])", RegexOptions.IgnoreCase);

    private ICoreServerAPI? sapi;

    public override bool ShouldLoad(EnumAppSide forSide) {
        return forSide.IsServer();
    }

    public override void StartServerSide(ICoreServerAPI api) {
        sapi = api;
        sapi.Event.PlayerChat += OnPlayerChat;
    }

    public override void Dispose() {
        if (sapi != null) {
            sapi.Event.PlayerChat -= OnPlayerChat;
        }
    }

    private static void OnPlayerChat(IServerPlayer sender, int channel, ref string message, ref string data, BoolRef consumed) {
        MatchCollection matches = ITEM_LINK.Matches(message);
        if (matches.Count == 0) {
            return;
        }

        int slotNum = sender.InventoryManager.ActiveHotbarSlotNumber;
        ItemStack itemStack = sender.InventoryManager.GetHotbarItemstack(slotNum);
        string pageCode = itemStack == null ? "" : GuiHandbookItemStackPage.PageCodeForStack(itemStack);

        string replacement = $"[{(pageCode is { Length: > 0 } ?
            $"<a href=\"handbook://{pageCode}\">{itemStack!.GetName()}</a>" :
            itemStack?.GetName() ?? Lang.Get("game:nothing"))}]";

        foreach (Match match in matches) {
            message = message.Replace(match.Value, replacement);
        }
    }
}
