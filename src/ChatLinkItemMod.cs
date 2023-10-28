using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace ChatLinkItem;

public class ChatLinkItemMod : ModSystem {
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
        if (!message.Contains("[item]")) {
            return;
        }

        int slotNum = sender.InventoryManager.ActiveHotbarSlotNumber;
        ItemStack itemStack = sender.InventoryManager.GetHotbarItemstack(slotNum);
        string pageCode = GuiHandbookItemStackPage.PageCodeForStack(itemStack);

        if (pageCode is { Length: > 0 }) {
            message = message.Replace("[item]", $"<a href=\"handbook://{pageCode}\">{itemStack.GetName()}</a>");
        }
    }
}
