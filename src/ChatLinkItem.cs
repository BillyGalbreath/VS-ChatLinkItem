using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace PlayerList;

public class ChatLinkItem : ModSystem {
    private ICoreServerAPI api;

    public override bool AllowRuntimeReload => true;

    public override bool ShouldLoad(EnumAppSide forSide) {
        return forSide.IsServer();
    }

    public override void StartServerSide(ICoreServerAPI api) {
        this.api = api;
        api.Event.PlayerChat += OnPlayerChat;
    }

    public override void Dispose() {
        api.Event.PlayerChat -= OnPlayerChat;
    }

    private void OnPlayerChat(IServerPlayer sender, int channel, ref string message, ref string data, BoolRef consumed) {
        if (!message.Contains("[item]")) {
            return;
        }

        int slotNum = sender.InventoryManager.ActiveHotbarSlotNumber;
        ItemStack itemStack = sender.InventoryManager.GetHotbarItemstack(slotNum);
        string pageCode = GuiHandbookItemStackPage.PageCodeForStack(itemStack);

        if (pageCode != null && pageCode.Length > 0) {
            message = message.Replace("[item]", $"<a href=\"handbook://{pageCode}\">{itemStack.GetName()}</a>");
        }
    }
}
