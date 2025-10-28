namespace QoL.Patches;

[HarmonyPatch(typeof(InventoryPaneInput), nameof(InventoryPaneInput.OnEnable))]
internal static class InventoryPaneInputPatch
{
    private static GameObject FleaCounter;
    private static TMProOld.TMP_Text Counter;

    [HarmonyWrapSafe, HarmonyPostfix]
    private static void Postfix_OnEnable()
    {
        if (!Configs.FleaTracked.Value)
        {
            if (FleaCounter != null) FleaCounter.SetActive(false);
            return;
        }

        if (FleaCounter == null)
        {
            GameObject obj = GameObject.Find("_GameCameras/HudCamera/In-game/Inventory/Inv").transform.GetChild(6).gameObject;
            FleaCounter = UObject.Instantiate(obj, obj.transform.GetParent(), false);
            FleaCounter.transform.localPosition = new(-8f, -15.25f, -3.3f);
            UObject.DestroyImmediate(FleaCounter.GetComponent<SetTextMeshProGameText>());
            FleaCounter.GetComponent<TMProOld.TMP_Text>().text = "Saved Fleas: ";
            var child = FleaCounter.transform.GetChild(0).gameObject;
            child.name = "Counter";
            Counter = child.GetComponent<TMProOld.TMP_Text>();
            FleaCounter.name = "FleaCounter";
            FleaCounter.SetActive(true);
        }

        if (!FleaCounter.activeSelf) FleaCounter.SetActive(true);

        int fleaCount = PlayerData.instance.SavedFleasCount;

        // Add Kratt
        if (PlayerData.instance.CaravanLechSaved) fleaCount++;

        // Add Vog
        if (PlayerData.instance.MetTroupeHunterWild) fleaCount++;

        // Add Huge Flea
        if (PlayerData.instance.tamedGiantFlea) fleaCount++;

        string temp = fleaCount.ToString();
        if (fleaCount >= 30)
            temp += " (Max)";

        Counter.text = temp;
    }
}
