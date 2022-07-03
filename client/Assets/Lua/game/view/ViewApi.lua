local API = {
    GameObject = {
        active = "active",
        activeSelf = "activeSelf",
        tag = "tag",
        clone = "clone",
        delete = "delete",
        name = "name"
    },
    Transform = {
        SetSiblingIndex = "SetSiblingIndex",
        localPosition = "localPosition",
        localEulerAngles = "localEulerAngles",
        localScale = "localScale",
        position = "position",
        parent = "parent",
        findchilddel = "objfindanddelete",
        screenPosition = "screenPosition"
    },
    RectTransform = {
        offsetMax = "offsetMax",
        offsetMin = "offsetMin",
        rectSize = "rectSize",
        sizeDelta = "sizeDelta",
        anchoredPosition = "anchoredPosition",
        anchoredPosition3D = "anchoredPosition3D",
        pivot = "pivot",
        anchorMax = "anchorMax",
        anchorMin = "anchorMin",
        SetInsetAndSizeFromParentEdge = "SetInsetAndSizeFromParentEdge",
        SetSizeWithCurrentAnchors = "SetSizeWithCurrentAnchors",
        RectangleContainsScreenPoint = "RectangleContainsScreenPoint",
        mousePosition = "mousePosition",
        transformPosition = "transformPosition",
        shaking = "shaking",
        localEulerAngles = "localEulerAngles",
        localScale = "localScale",
        transformLocalPosFromOther = "transformLocalPosFromOther",
        isOverlaps = "isOverlaps",
        forceRebuildLayoutForRect = "forceRebuildLayoutForRect"
    },
    VerticalLayoutGroup = {
        spacing = "spacing",
    },
    Button = {
        onClick = "onClick",
        interactable = "interactable"
    },
    DropDown = {
        clearOptions = "clearOptions",
        addOptions = "addOptions",
        onValueChanged = "onValueChanged",
        value = "value"
    },
    Toggle = {
        onValueChanged = "onValueChanged",
        interactable = "interactable",
        on = "on",
        group = "group"
    },
    ToggleGroup = {
        allowSwitchOff = "allowSwitchOff"
    },
    Graphic = {
        SetNativeSize = "SetNativeSize",
        color = "color",
        raycastTarget = "raycastTarget",
        gray = "gray"
    },
    Text = {
        text = "text",
        resize = "resize",
        fontSize = "fontSize",
        alignment = "alignment",
        color = "color",
    },
    Outline = {
        effectColor = "effectColor",
        effectDistance = "effectDistance",
    },
    InputField = {
        text = "text",
        activateInputField = "activateInputField",
        onValueChanged = "onValueChanged",
        onEndEdit = "onEndEdit"
    },
    Slider = {
        value = "value"
    },
    ScrollRect = {
        onValueChanged = "onValueChanged",
        horizontalNormalizedPosition = "horizontalNormalizedPosition",
        verticalNormalizedPosition = "verticalNormalizedPosition",
        velocity = "velocity",
        stopMovement = "stopMovement"
    },
    Scrollbar = {
        value = "value"
    },
    Tween = {
        DoLocalMove = "DoLocalMove",
        DoLocalMoveX = "DoLocalMoveX",
        DoLocalMoveY = "DoLocalMoveY",
        DoLocalMoveZ = "DoLocalMoveZ",
        DoMove = "DoMove",
        DoMoveX = "DoMoveX",
        DoMoveY = "DoMoveY",
        DoMoveZ = "DoMoveZ",
        DoLocalMoveLoop = "DoLocalMoveLoop",
        DoLocalMoveLoopX = "DoLocalMoveLoopX",
        DoLocalMoveLoopY = "DoLocalMoveLoopY",
        DoLocalMoveLoopZ = "DoLocalMoveLoopZ",
        DOScale = "DOScale",
        DOScaleX = "DOScaleX",
        DOScaleY = "DOScaleY",
        DOScaleZ = "DOScaleZ",
        DOLocalRotate = "DOLocalRotate",
        DOLocalRotateLinear = "DOLocalRotateLinear",
        DOFade = "DOFade",
        DOFadeLoop = "DOFadeLoop",
        DOLocalRotateLoop = "DOLocalRotateLoop",
        DOAnchorMove = "DOAnchorMove",
        DOAnchorMoveX = "DOAnchorMoveX",
        DOAnchorMoveY = "DOAnchorMoveY",
        DOAnchorMoveZ = "DOAnchorMoveZ",
    },
    ListView = {
        init = "OnInit",
        validate = "OnValidate",
        itemcount = "OnItemCount",
        addItemToTop = "OnAddItemToTop",
        addItemToBottom = "OnAddItemToBottom",
        addItem = "OnAddItem",
        removeItem = "OnRemoveItem",
        removeTop = "OnRemoveTop",
        removeBottom = "OnRemoveBottom",
        removeAllItems = "OnRemoveAllItems",
        getItem = "OnGetItem",
        findItem = "OnFindItem",
        findItems = "OnFindItems",
        locateTo = "OnLocateTo",
        sort = "OnSort",
    },
    Image = {
        sprite = "sprite",
        native = "native",
        fillAmount = "fillAmount",
        texture = "texture",
        resize = "resize",
        loadAtlas = "loadAtlas",
        color = "color",
    },
    RawImage = {
        asset = "asset",
        texture = "texture",
        raycastTarget = "raycastTarget"
    },
    Canvas = {
        encodeToPNG = "encodeToPNG",
        init = "init",
        planeDistance = "planeDistance",
        order = "order",
        sortingLayerName = "sortingLayerName"
    },
    CanvasGroup = {
        alpha = "alpha",
        interactable = "interactable",
    }
}

return API
