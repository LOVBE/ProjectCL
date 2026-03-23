using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private readonly Color panelColor = new Color(0.05f, 0.07f, 0.1f, 0.82f);
    private readonly Color accentColor = new Color(0.31f, 0.86f, 0.76f, 1f);
    private readonly Color staminaColor = new Color(0.95f, 0.58f, 0.18f, 1f);
    private readonly Color oxygenColor = new Color(0.3f, 0.74f, 1f, 1f);
    private readonly Color heatColor = new Color(0.96f, 0.35f, 0.18f, 1f);

    private Canvas rootCanvas;
    private Font defaultFont;
    private Sprite whiteSprite;

    private GameObject explorationUI;
    private GameObject craftingUI;
    private GameObject endOfDayUI;
    private GameObject gameOverUI;

    private Image staminaFill;
    private Text staminaText;
    private Image oxygenFill;
    private Text oxygenText;
    private Text hudGoldText;
    private Text hudDayText;
    private Text hudTaxText;

    private Text craftingTemperatureText;
    private Image craftingTemperatureFill;
    private Text craftingFireText;
    private Text craftingIngredientsText;
    private Text craftingInventoryEmptyText;
    private RectTransform craftingInventoryContent;
    private Text fireButtonText;

    private Text endDayHeaderText;
    private Text endDayGoldText;
    private Text endDayTaxText;
    private Text endDayReasonText;

    private Text gameOverReasonText;

    private string lastInventorySignature = string.Empty;
    private Text langButtonText;

    private void Awake()
    {
        EnsureEventSystem();
        BuildRuntimeUi();
        Loc.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        Loc.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        RebuildAllText();
    }

    private void Update()
    {
        if (rootCanvas == null)
        {
            BuildRuntimeUi();
        }

        RefreshUi();
    }

    public void OnClickNextDay()
    {
        GameManager.Instance?.StartNextDay();
    }

    private void BuildRuntimeUi()
    {
        if (rootCanvas != null)
        {
            return;
        }

        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        whiteSprite = BuildWhiteSprite();

        GameObject canvasObject = new GameObject("RuntimeUI_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        rootCanvas = canvasObject.GetComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = 500;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        explorationUI = CreateScreenRoot("ExplorationUI", rootCanvas.transform);
        craftingUI = CreateScreenRoot("CraftingUI", rootCanvas.transform);
        endOfDayUI = CreateScreenRoot("EndOfDayUI", rootCanvas.transform);
        gameOverUI = CreateScreenRoot("GameOverUI", rootCanvas.transform);

        BuildExplorationHud(explorationUI.transform);
        BuildCraftingUi(craftingUI.transform);
        BuildEndOfDayUi(endOfDayUI.transform);
        BuildGameOverUi(gameOverUI.transform);

        RefreshUi();
    }

    private void BuildExplorationHud(Transform parent)
    {
        RectTransform vitalsPanel = CreatePanel("VitalsPanel", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(360f, 150f));
        CreateLabel("VitalsTitle", vitalsPanel, "Tham hiem", 24, TextAnchor.UpperLeft, new Vector2(16f, -12f), new Vector2(220f, 28f));

        staminaText = CreateLabel("StaminaText", vitalsPanel, Loc.Get("hud_stamina", 100, 100), 18, TextAnchor.MiddleLeft, new Vector2(16f, -48f), new Vector2(328f, 22f));
        staminaFill = CreateBar("StaminaBar", vitalsPanel, staminaColor, new Vector2(16f, -78f), new Vector2(328f, 18f));

        oxygenText = CreateLabel("OxygenText", vitalsPanel, Loc.Get("hud_oxygen", 100, 100), 18, TextAnchor.MiddleLeft, new Vector2(16f, -98f), new Vector2(328f, 22f));
        oxygenFill = CreateBar("OxygenBar", vitalsPanel, oxygenColor, new Vector2(16f, -128f), new Vector2(328f, 18f));

        RectTransform economyPanel = CreatePanel("EconomyPanel", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(300f, 140f));
        hudDayText = CreateLabel("HudDayText", economyPanel, Loc.Get("hud_day", 1), 20, TextAnchor.UpperRight, new Vector2(16f, -16f), new Vector2(268f, 24f));
        hudGoldText = CreateLabel("HudGoldText", economyPanel, Loc.Get("hud_gold", 0), 18, TextAnchor.UpperRight, new Vector2(16f, -52f), new Vector2(268f, 24f));
        hudTaxText = CreateLabel("HudTaxText", economyPanel, Loc.Get("hud_tax", 7), 18, TextAnchor.UpperRight, new Vector2(16f, -88f), new Vector2(268f, 24f));

        langButtonText = CreateButton("LangSwitchButton", economyPanel, Loc.Get("lang_switch"), OnLangSwitchClicked, new Vector2(120f, 32f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-8f, 8f));

        RectTransform crosshairRoot = CreateRect("Crosshair", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(28f, 28f));
        Image horizontal = CreateImage("Horizontal", crosshairRoot, accentColor, Vector2.zero, new Vector2(16f, 2f));
        horizontal.raycastTarget = false;
        Image vertical = CreateImage("Vertical", crosshairRoot, accentColor, Vector2.zero, new Vector2(2f, 16f));
        vertical.raycastTarget = false;
    }

    private void BuildCraftingUi(Transform parent)
    {
        CreateFullscreenBackdrop("CraftingBackdrop", parent, new Color(0f, 0f, 0f, 0.55f));
        RectTransform mainPanel = CreatePanel("CraftingPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1080f, 620f));

        CreateLabel("CraftingTitle", mainPanel, Loc.Get("craft_title"), 30, TextAnchor.UpperLeft, new Vector2(28f, -22f), new Vector2(460f, 36f));
        CreateLabel("CraftingHint", mainPanel, Loc.Get("craft_hint"), 18, TextAnchor.UpperLeft, new Vector2(30f, -64f), new Vector2(520f, 24f));

        RectTransform inventoryPanel = CreatePanel("InventoryPanel", mainPanel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -104f), new Vector2(360f, 396f));
        CreateLabel("InventoryTitle", inventoryPanel, Loc.Get("craft_inv_title"), 22, TextAnchor.UpperLeft, new Vector2(18f, -16f), new Vector2(300f, 28f));

        RectTransform inventoryViewport = CreateRect("InventoryViewport", inventoryPanel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -62f), new Vector2(324f, 300f));
        inventoryViewport.pivot = new Vector2(0f, 1f);
        VerticalLayoutGroup inventoryLayout = inventoryViewport.gameObject.AddComponent<VerticalLayoutGroup>();
        inventoryLayout.spacing = 10f;
        inventoryLayout.padding = new RectOffset(0, 0, 0, 0);
        inventoryLayout.childAlignment = TextAnchor.UpperCenter;
        inventoryLayout.childControlHeight = false;
        inventoryLayout.childControlWidth = true;
        inventoryLayout.childForceExpandHeight = false;
        inventoryLayout.childForceExpandWidth = true;

        ContentSizeFitter inventoryFitter = inventoryViewport.gameObject.AddComponent<ContentSizeFitter>();
        inventoryFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        inventoryFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        craftingInventoryContent = inventoryViewport;
        craftingInventoryEmptyText = CreateLabel("InventoryEmpty", inventoryPanel, Loc.Get("craft_inv_empty"), 18, TextAnchor.MiddleCenter, new Vector2(18f, -180f), new Vector2(324f, 80f));

        RectTransform statusPanel = CreatePanel("StatusPanel", mainPanel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(412f, -104f), new Vector2(644f, 396f));
        craftingTemperatureText = CreateLabel("TemperatureText", statusPanel, Loc.Get("craft_temp", 20), 24, TextAnchor.UpperLeft, new Vector2(22f, -20f), new Vector2(460f, 30f));
        craftingTemperatureFill = CreateBar("TemperatureBar", statusPanel, heatColor, new Vector2(22f, -58f), new Vector2(600f, 22f));
        craftingFireText = CreateLabel("FireText", statusPanel, Loc.Get("craft_fire", Loc.Get("craft_fire_off")), 20, TextAnchor.UpperLeft, new Vector2(22f, -94f), new Vector2(320f, 26f));
        CreateLabel("IngredientsTitle", statusPanel, Loc.Get("craft_cauldron_title"), 22, TextAnchor.UpperLeft, new Vector2(22f, -142f), new Vector2(320f, 28f));
        craftingIngredientsText = CreateLabel("IngredientsText", statusPanel, Loc.Get("craft_cauldron_empty"), 18, TextAnchor.UpperLeft, new Vector2(22f, -180f), new Vector2(620f, 180f));
        craftingIngredientsText.verticalOverflow = VerticalWrapMode.Overflow;
        CreateLabel("CraftingFooterNote", statusPanel, Loc.Get("craft_footer"), 16, TextAnchor.LowerLeft, new Vector2(22f, -16f), new Vector2(620f, 26f));

        RectTransform buttonRow = CreateRect("ButtonRow", mainPanel, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-24f, 24f), new Vector2(720f, 56f));
        buttonRow.pivot = new Vector2(1f, 0f);
        HorizontalLayoutGroup buttonLayout = buttonRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        buttonLayout.spacing = 14f;
        buttonLayout.childAlignment = TextAnchor.MiddleRight;
        buttonLayout.childControlWidth = false;
        buttonLayout.childControlHeight = false;
        buttonLayout.childForceExpandWidth = false;
        buttonLayout.childForceExpandHeight = false;

        fireButtonText = CreateButton("ToggleFireButton", buttonRow, Loc.Get("btn_toggle_fire"), OnToggleFireClicked, new Vector2(220f, 48f));
        CreateButton("ReturnExplorationButton", buttonRow, Loc.Get("btn_return_explore"), OnReturnToExplorationClicked, new Vector2(240f, 48f));
        CreateButton("EndDayButton", buttonRow, Loc.Get("btn_end_day"), OnEndDayClicked, new Vector2(220f, 48f));
    }

    private void BuildEndOfDayUi(Transform parent)
    {
        CreateFullscreenBackdrop("EndDayBackdrop", parent, new Color(0f, 0f, 0f, 0.72f));
        RectTransform panel = CreatePanel("EndDayPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 430f));

        endDayHeaderText = CreateAnchoredLabel("EndDayHeader", panel, Loc.Get("end_header", 1), 34, TextAnchor.UpperCenter, new Vector2(0f, -26f), new Vector2(640f, 40f), Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        endDayGoldText = CreateLabel("EndDayGold", panel, Loc.Get("end_gold", 0), 22, TextAnchor.UpperLeft, new Vector2(42f, -112f), new Vector2(620f, 30f));
        endDayTaxText = CreateLabel("EndDayTax", panel, Loc.Get("end_tax", 7), 22, TextAnchor.UpperLeft, new Vector2(42f, -156f), new Vector2(620f, 30f));
        endDayReasonText = CreateLabel("EndDayReason", panel, Loc.Get("end_no_incident"), 20, TextAnchor.UpperLeft, new Vector2(42f, -214f), new Vector2(620f, 90f));
        endDayReasonText.verticalOverflow = VerticalWrapMode.Overflow;

        CreateButton("NextDayButton", panel, Loc.Get("btn_next_day"), OnClickNextDay, new Vector2(260f, 52f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 28f));
    }

    private void BuildGameOverUi(Transform parent)
    {
        CreateFullscreenBackdrop("GameOverBackdrop", parent, new Color(0.1f, 0f, 0f, 0.84f));
        RectTransform panel = CreatePanel("GameOverPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700f, 300f), new Color(0.2f, 0.04f, 0.04f, 0.94f));

        CreateAnchoredLabel("GameOverTitle", panel, Loc.Get("gameover_title"), 40, TextAnchor.UpperCenter, new Vector2(0f, -28f), new Vector2(620f, 44f), new Color(1f, 0.82f, 0.82f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        gameOverReasonText = CreateAnchoredLabel("GameOverReason", panel, Loc.Get("gameover_reason"), 22, TextAnchor.MiddleCenter, new Vector2(0f, -20f), new Vector2(600f, 110f), Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
    }

    private void RefreshUi()
    {
        string stateName = GetCurrentStateName();
        bool isGameOver = stateName == "GameOver" || ReadBoolMember(GameManager.Instance, "IsGameOver", false);

        if (explorationUI != null) explorationUI.SetActive(stateName == GameManager.GameState.Exploration.ToString() && !isGameOver);
        if (craftingUI != null) craftingUI.SetActive(stateName == GameManager.GameState.Crafting.ToString() && !isGameOver);
        if (endOfDayUI != null) endOfDayUI.SetActive(stateName == GameManager.GameState.Management.ToString() && !isGameOver);
        if (gameOverUI != null) gameOverUI.SetActive(isGameOver);

        RefreshHud();
        RefreshCraftingPanel();
        RefreshEndOfDayPanel();
        RefreshGameOverPanel();
    }

    private void RefreshHud()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        float currentStamina = ReadFloatMember(player, "CurrentStamina", ReadFloatMember(player, "currentStamina", 0f));
        float maxStamina = Mathf.Max(1f, ReadFloatMember(player, "MaxStamina", ReadFloatMember(player, "maxStamina", 100f)));
        float currentOxygen = ReadFloatMember(player, "CurrentOxygen", ReadFloatMember(player, "currentOxygen", 0f));
        float maxOxygen = Mathf.Max(1f, ReadFloatMember(player, "MaxOxygen", ReadFloatMember(player, "maxOxygen", 100f)));

        UpdateBar(staminaFill, currentStamina / maxStamina);
        UpdateBar(oxygenFill, currentOxygen / maxOxygen);

        if (staminaText != null) staminaText.text = Loc.Get("hud_stamina", Mathf.RoundToInt(currentStamina), Mathf.RoundToInt(maxStamina));
        if (oxygenText != null) oxygenText.text = Loc.Get("hud_oxygen", Mathf.RoundToInt(currentOxygen), Mathf.RoundToInt(maxOxygen));
        if (hudGoldText != null) hudGoldText.text = Loc.Get("hud_gold", GetCurrentGold());
        if (hudDayText != null) hudDayText.text = Loc.Get("hud_day", GetCurrentDay());
        if (hudTaxText != null) hudTaxText.text = Loc.Get("hud_tax", GetDaysToTax());
    }

    private void RefreshCraftingPanel()
    {
        Cauldron cauldron = ResolveCauldron();
        float currentTemperature = ReadFloatMember(cauldron, "CurrentTemperature", ReadFloatMember(cauldron, "currentTemperature", 20f));
        float maxSafeTemperature = Mathf.Max(1f, ReadFloatMember(cauldron, "MaxSafeTemperature", ReadFloatMember(cauldron, "maxSafeTemperature", 150f)));
        bool isFireOn = ReadBoolMember(cauldron, "IsFireOn", ReadBoolMember(cauldron, "isFireOn", false));

        if (craftingTemperatureText != null)
        {
            craftingTemperatureText.text = cauldron == null
                ? Loc.Get("craft_temp", "--")
                : Loc.Get("craft_temp", $"{currentTemperature:0.#} / {maxSafeTemperature:0.#}");
        }

        if (craftingFireText != null)
        {
            craftingFireText.text = Loc.Get("craft_fire", isFireOn ? Loc.Get("craft_fire_on") : Loc.Get("craft_fire_off"));
        }

        if (fireButtonText != null)
        {
            fireButtonText.text = isFireOn ? Loc.Get("btn_fire_on") : Loc.Get("btn_fire_off");
        }

        UpdateBar(craftingTemperatureFill, currentTemperature / maxSafeTemperature);
        RefreshCauldronIngredients(cauldron);
        RefreshInventoryButtons();
    }

    private void RefreshCauldronIngredients(Cauldron cauldron)
    {
        if (craftingIngredientsText == null)
        {
            return;
        }

        if (cauldron == null)
        {
            craftingIngredientsText.text = Loc.Get("craft_cauldron_no_mgr");
            return;
        }

        List<ItemID> ingredients = ExtractIngredients(ReadRawMember(cauldron, "CurrentIngredients") ?? ReadRawMember(cauldron, "currentIngredients"));
        if (ingredients.Count == 0)
        {
            craftingIngredientsText.text = Loc.Get("craft_cauldron_empty");
            return;
        }

        Dictionary<ItemID, int> counts = new Dictionary<ItemID, int>();
        foreach (ItemID item in ingredients)
        {
            if (counts.ContainsKey(item)) counts[item]++;
            else counts[item] = 1;
        }

        List<string> lines = new List<string>();
        foreach (KeyValuePair<ItemID, int> entry in counts)
        {
            lines.Add($"- {GetItemDisplayName(entry.Key)} x{entry.Value}");
        }

        craftingIngredientsText.text = string.Join("\n", lines);
    }

    private void RefreshInventoryButtons()
    {
        if (craftingInventoryContent == null)
        {
            return;
        }

        Dictionary<ItemID, int> inventory = GetInventorySnapshot();
        List<KeyValuePair<ItemID, int>> ingredients = new List<KeyValuePair<ItemID, int>>();
        foreach (KeyValuePair<ItemID, int> entry in inventory)
        {
            if (entry.Value > 0 && CanUseAsIngredient(entry.Key))
            {
                ingredients.Add(entry);
            }
        }

        ingredients.Sort((left, right) => left.Key.CompareTo(right.Key));

        string signature = BuildInventorySignature(ingredients);
        if (signature == lastInventorySignature)
        {
            if (craftingInventoryEmptyText != null)
            {
                craftingInventoryEmptyText.gameObject.SetActive(ingredients.Count == 0);
            }
            return;
        }

        lastInventorySignature = signature;

        for (int i = craftingInventoryContent.childCount - 1; i >= 0; i--)
        {
            Destroy(craftingInventoryContent.GetChild(i).gameObject);
        }

        if (craftingInventoryEmptyText != null)
        {
            craftingInventoryEmptyText.gameObject.SetActive(ingredients.Count == 0);
        }

        foreach (KeyValuePair<ItemID, int> entry in ingredients)
        {
            ItemID localItem = entry.Key;
            CreateButton(
                $"Ingredient_{localItem}",
                craftingInventoryContent,
                $"{GetItemDisplayName(localItem)} x{entry.Value}",
                () => OnIngredientClicked(localItem),
                new Vector2(0f, 46f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                Vector2.zero);
        }
    }

    private void RefreshEndOfDayPanel()
    {
        int lastCompletedDay = ReadIntMember(GameManager.Instance, "LastCompletedDayCount", GetCurrentDay());
        string faintReason = ReadStringMember(GameManager.Instance, "LastFaintReason", string.Empty);
        int faintPenalty = ReadIntMember(GameManager.Instance, "LastFaintGoldPenalty", 0);

        if (endDayHeaderText != null) endDayHeaderText.text = Loc.Get("end_header", lastCompletedDay);
        if (endDayGoldText != null) endDayGoldText.text = Loc.Get("end_gold", GetCurrentGold());
        if (endDayTaxText != null) endDayTaxText.text = Loc.Get("end_tax", GetDaysToTax());

        if (endDayReasonText != null)
        {
            endDayReasonText.text = string.IsNullOrEmpty(faintReason)
                ? Loc.Get("end_no_incident")
                : faintPenalty > 0
                    ? $"{faintReason}\n(-{faintPenalty} gold)"
                    : faintReason;
        }
    }

    private void RefreshGameOverPanel()
    {
        if (gameOverReasonText == null)
        {
            return;
        }

        string reason = ReadStringMember(GameManager.Instance, "GameOverReason", string.Empty);
        if (string.IsNullOrEmpty(reason))
        {
            reason = Loc.Get("gameover_reason");
        }

        gameOverReasonText.text = reason;
    }

    private void OnIngredientClicked(ItemID itemID)
    {
        if (AlchemyManager.Instance != null)
        {
            AlchemyManager.Instance.TryDropItemIntoCauldron(itemID);
        }
    }

    private void OnLangSwitchClicked()
    {
        Loc.Toggle();
    }

    /// <summary>
    /// Hủy toàn bộ Canvas cũ và dựng lại từ đầu với ngôn ngữ mới.
    /// </summary>
    private void RebuildAllText()
    {
        if (rootCanvas != null)
        {
            Destroy(rootCanvas.gameObject);
            rootCanvas = null;
        }

        // Reset cache
        staminaFill = null; staminaText = null;
        oxygenFill = null; oxygenText = null;
        hudGoldText = null; hudDayText = null; hudTaxText = null;
        craftingTemperatureText = null; craftingTemperatureFill = null;
        craftingFireText = null; craftingIngredientsText = null;
        craftingInventoryEmptyText = null; craftingInventoryContent = null;
        fireButtonText = null; langButtonText = null;
        endDayHeaderText = null; endDayGoldText = null;
        endDayTaxText = null; endDayReasonText = null;
        gameOverReasonText = null;
        lastInventorySignature = string.Empty;

        BuildRuntimeUi();
    }

    private void OnToggleFireClicked()
    {
        object alchemyManager = AlchemyManager.Instance;
        MethodInfo method = alchemyManager != null
            ? alchemyManager.GetType().GetMethod("ToggleMainCauldronFire", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : null;

        if (method != null)
        {
            method.Invoke(alchemyManager, null);
            return;
        }

        Cauldron cauldron = ResolveCauldron();
        if (cauldron != null)
        {
            cauldron.ToggleFire();
        }
    }

    private void OnReturnToExplorationClicked()
    {
        if (TryInvokeAlchemyManagerMethod("ReturnToExploration"))
        {
            return;
        }

        GameManager.Instance?.ChangeState(GameManager.GameState.Exploration);
    }

    private void OnEndDayClicked()
    {
        if (TryInvokeAlchemyManagerMethod("EndCraftingDay"))
        {
            return;
        }

        GameManager.Instance?.ChangeState(GameManager.GameState.Management);
    }

    private int GetCurrentGold()
    {
        return ReadIntMember(EconomyManager.Instance, "CurrentGold", 0);
    }

    private string GetCurrentStateName()
    {
        object state = ReadRawMember(GameManager.Instance, "CurrentState");
        return state != null ? state.ToString() : GameManager.GameState.Exploration.ToString();
    }

    private int GetCurrentDay()
    {
        return ReadIntMember(GameManager.Instance, "CurrentDayCount", 1);
    }

    private int GetDaysToTax()
    {
        EconomyManager economy = EconomyManager.Instance;
        int days = ReadIntMember(economy, "DaysToTax", int.MinValue);
        if (days != int.MinValue)
        {
            return days;
        }

        days = ReadIntMember(economy, "daysToTax", int.MinValue);
        if (days != int.MinValue)
        {
            return days;
        }

        return ReadIntMember(economy, "TaxCycleDays", 0);
    }

    private Cauldron ResolveCauldron()
    {
        object alchemyManager = AlchemyManager.Instance;
        object cauldronObject = ReadRawMember(alchemyManager, "MainCauldron") ?? ReadRawMember(alchemyManager, "mainCauldron");
        if (cauldronObject is Cauldron cauldron)
        {
            return cauldron;
        }

        return FindObjectOfType<Cauldron>();
    }

    private Dictionary<ItemID, int> GetInventorySnapshot()
    {
        Dictionary<ItemID, int> snapshot = new Dictionary<ItemID, int>();
        object inventorySource = ReadRawMember(PlayerInventory.Instance, "Items") ?? ReadRawMember(PlayerInventory.Instance, "inventory");

        if (inventorySource is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key is ItemID itemId)
                {
                    snapshot[itemId] = Convert.ToInt32(entry.Value);
                }
            }

            return snapshot;
        }

        if (inventorySource is IEnumerable enumerable)
        {
            foreach (object entry in enumerable)
            {
                object key = ReadRawMember(entry, "Key");
                object value = ReadRawMember(entry, "Value");
                if (key is ItemID itemId)
                {
                    snapshot[itemId] = Convert.ToInt32(value);
                }
            }
        }

        return snapshot;
    }

    private bool CanUseAsIngredient(ItemID itemID)
    {
        object alchemyManager = AlchemyManager.Instance;
        MethodInfo canUseMethod = alchemyManager != null
            ? alchemyManager.GetType().GetMethod("CanUseAsIngredient", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : null;

        if (canUseMethod != null)
        {
            object result = canUseMethod.Invoke(alchemyManager, new object[] { itemID });
            if (result is bool canUse)
            {
                return canUse;
            }
        }

        return IsIngredient(itemID);
    }

    private bool TryInvokeAlchemyManagerMethod(string methodName)
    {
        object alchemyManager = AlchemyManager.Instance;
        MethodInfo method = alchemyManager != null
            ? alchemyManager.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : null;

        if (method == null)
        {
            return false;
        }

        method.Invoke(alchemyManager, null);
        return true;
    }

    private List<ItemID> ExtractIngredients(object source)
    {
        List<ItemID> result = new List<ItemID>();
        if (source is IEnumerable enumerable)
        {
            foreach (object entry in enumerable)
            {
                if (entry is ItemID itemId)
                {
                    result.Add(itemId);
                }
            }
        }

        return result;
    }

    private static object ReadRawMember(object target, string memberName)
    {
        if (target == null || string.IsNullOrEmpty(memberName))
        {
            return null;
        }

        Type type = target.GetType();
        PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null ? field.GetValue(target) : null;
    }

    private static int ReadIntMember(object target, string memberName, int fallback)
    {
        object value = ReadRawMember(target, memberName);
        return value != null ? Convert.ToInt32(value) : fallback;
    }

    private static float ReadFloatMember(object target, string memberName, float fallback)
    {
        object value = ReadRawMember(target, memberName);
        return value != null ? Convert.ToSingle(value) : fallback;
    }

    private static bool ReadBoolMember(object target, string memberName, bool fallback)
    {
        object value = ReadRawMember(target, memberName);
        return value != null ? Convert.ToBoolean(value) : fallback;
    }

    private static string ReadStringMember(object target, string memberName, string fallback)
    {
        object value = ReadRawMember(target, memberName);
        return value != null ? value.ToString() : fallback;
    }

    private static string BuildInventorySignature(List<KeyValuePair<ItemID, int>> items)
    {
        if (items.Count == 0)
        {
            return "empty";
        }

        List<string> chunks = new List<string>();
        foreach (KeyValuePair<ItemID, int> item in items)
        {
            chunks.Add($"{item.Key}:{item.Value}");
        }

        return string.Join("|", chunks);
    }

    private static bool IsIngredient(ItemID itemID)
    {
        switch (itemID)
        {
            case ItemID.GlowingMushroom:
            case ItemID.SmellyRoot:
            case ItemID.IceMint:
            case ItemID.MischiefBerry:
                return true;
            default:
                return false;
        }
    }

    private static string GetItemDisplayName(ItemID itemID)
    {
        switch (itemID)
        {
            case ItemID.GlowingMushroom:
                return "Nam phat quang";
            case ItemID.SmellyRoot:
                return "Re cay boc mui";
            case ItemID.IceMint:
                return "La bac ha bang";
            case ItemID.MischiefBerry:
                return "Qua mong tinh nghich";
            case ItemID.SilentStepPotion:
                return "Thuoc buoc chan em";
            case ItemID.MeatBombPotion:
                return "Bom mui thit nuong";
            case ItemID.HeatResistPotion:
                return "Thuoc khang nhiet";
            case ItemID.SmokePowder:
                return "Bot khoi mu";
            default:
                return itemID.ToString();
        }
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("RuntimeUI_EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystemObject.transform.SetParent(transform, false);
    }

    private Sprite BuildWhiteSprite()
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.name = "RuntimeUI_White";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.hideFlags = HideFlags.HideAndDontSave;
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
    }

    private GameObject CreateScreenRoot(string name, Transform parent)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return rect.gameObject;
    }

    private RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        return CreatePanel(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta, panelColor);
    }

    private RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = whiteSprite;
        image.type = Image.Type.Simple;
        image.color = color;
        return rect;
    }

    private Image CreateFullscreenBackdrop(string name, Transform parent, Color color)
    {
        Image image = CreateImage(name, parent, color, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
        image.raycastTarget = true;
        return image;
    }

    private RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);

        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(anchorMin.x == anchorMax.x ? anchorMin.x : 0.5f, anchorMin.y == anchorMax.y ? anchorMin.y : 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        return rect;
    }

    private Text CreateLabel(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        return CreateLabel(name, parent, content, fontSize, alignment, anchoredPosition, sizeDelta, Color.white);
    }

    private Text CreateLabel(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
    {
        return CreateAnchoredLabel(name, parent, content, fontSize, alignment, anchoredPosition, sizeDelta, color, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
    }

    private Text CreateAnchoredLabel(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Vector2 anchoredPosition, Vector2 sizeDelta, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta);
        rect.pivot = pivot;

        Text text = rect.gameObject.AddComponent<Text>();
        text.font = defaultFont;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.text = content;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private Image CreateBar(string name, Transform parent, Color fillColor, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform backgroundRect = CreateRect(name, parent, new Vector2(0f, 1f), new Vector2(0f, 1f), anchoredPosition, sizeDelta);
        backgroundRect.pivot = new Vector2(0f, 1f);

        Image background = backgroundRect.gameObject.AddComponent<Image>();
        background.sprite = whiteSprite;
        background.color = new Color(1f, 1f, 1f, 0.12f);

        RectTransform fillRect = CreateRect("Fill", backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fill = fillRect.gameObject.AddComponent<Image>();
        fill.sprite = whiteSprite;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 1f;
        fill.color = fillColor;
        return fill;
    }

    private Text CreateButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction onClick, Vector2 sizeDelta)
    {
        return CreateButton(name, parent, label, onClick, sizeDelta, new Vector2(0f, 0f), new Vector2(0f, 0f), Vector2.zero);
    }

    private Text CreateButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction onClick, Vector2 sizeDelta, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta);

        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = whiteSprite;
        image.color = accentColor;

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        ColorBlock colors = button.colors;
        colors.normalColor = accentColor;
        colors.highlightedColor = accentColor * 1.06f;
        colors.pressedColor = accentColor * 0.88f;
        colors.selectedColor = accentColor;
        colors.disabledColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.4f);
        button.colors = colors;

        RectTransform textRect = CreateRect("Label", rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        textRect.offsetMin = new Vector2(10f, 6f);
        textRect.offsetMax = new Vector2(-10f, -6f);

        Text text = textRect.gameObject.AddComponent<Text>();
        text.font = defaultFont;
        text.fontSize = 18;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.04f, 0.08f, 0.12f, 1f);
        text.text = label;

        LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
        if (Mathf.Abs(sizeDelta.x) > 0.01f) layout.preferredWidth = sizeDelta.x;
        if (Mathf.Abs(sizeDelta.y) > 0.01f) layout.preferredHeight = sizeDelta.y;

        return text;
    }

    private Image CreateImage(string name, Transform parent, Color color, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        return CreateImage(name, parent, color, anchoredPosition, sizeDelta, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
    }

    private Image CreateImage(string name, Transform parent, Color color, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 anchorMin, Vector2 anchorMax)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = whiteSprite;
        image.color = color;
        return image;
    }

    private static void UpdateBar(Image fillImage, float normalizedValue)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
        }
    }
}
