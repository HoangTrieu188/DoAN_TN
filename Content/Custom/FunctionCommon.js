function Toolbar_Click(e) {
    switch (e) {
        case "btnSearch": VoidSearch(); break;
        case "btnAdd": Create('0'); break;
        case "btnEdit": Edit('1'); break;
        case "btnDelete": Delete(); break;
        case "btnRefresh": voidRefresh(); break;
        case "btnCopy": Copy(); break;
        case "btnExportxlsx": Exportxlsx(); break;
        case "btnExportpdf": Exportpdf(); break;
        case "btnImport": Import(); break;

        case "btnDisplay": Display(); break;
        case "btnConfirmOrder": ConfirmOrder(); break;
        case "btnRejectOrder": RejectOrder(); break;
    }
}

function Toolbarc_Click(e) {
    switch (e) {
        case "txtSearchc": VoidTxtSearchc(); break;
        case "btnDeletec": VoidDeleteTabc(); break;
        case "btnAddc": CreateTab(); break;
        case "btnEditc": EditTab(); break;
        case "btnImportc": ImportTab(); break;
        case "btnCopyc": CopyTab(); break;
    }
}

function ToolbarItem_Click(e) {
    switch (e) {
        case "btnSaveadd":
            {
                document.getElementById("hfLuuvathem").value = "1";
                document.getElementById("btnLuu").click();
                break;
            }

        case "btnSaveclose":
            {
                document.getElementById("hfLuuvathem").value = "0";
                document.getElementById("btnLuu").click();
                break;
            }
        case "btnDelete": voidDeleteItem();
            break;
        case "btnRefresh": voidRefreshItem();
            break;
        case "btnClose":
            closeWindowEdit();
            break;
    }
}

function RemoveUnicode(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_|–|”|“|`/g, "-");
    str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
}

function PreventEventTextSubmit() {
    $(document).ready(function () {
        $("input:text").keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
    });
}

function KeyboardNavigationEdit() {
    $(document.body).keyup(function (e) {
        //Esc - thoat
        if (e.keyCode == 27) {
            ToolbarItem_Click("btnClose");
        }
    });
}

function KendoTabStrip() {
    $("#Tabs").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
}

function GridHeight(idGrid) {
    var gridElement = $("#" + idGrid + "");
    var dataArea = gridElement.find(".k-grid-content");
    var gridHeight = $(window).innerHeight() - 60;//
    var otherElements = gridElement.children().not(".k-grid-content");
    var otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(gridHeight - otherElementsHeight);
}

function GridHeightTab(idGrid, height) {
    var gridElement = $("#" + idGrid + "");
    var dataArea = gridElement.find(".k-grid-content");
    var gridHeight = $(window).innerHeight() - (60 + height);//
    var otherElements = gridElement.children().not(".k-grid-content");
    var otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(gridHeight - otherElementsHeight);
}

function commaSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    return val;
}

function Notification(title, statusCode, messagError, statusNotification, isEdit) {
    var tmpMessage;
    var tmpIdNotification;
    if (statusCode) {
        tmpMessage = "StatusCode:" + statusCode + " </br> " + messagError;
    } else {
        tmpMessage = messagError;
    }
    if (isEdit) {
        tmpIdNotification = "#notificationEdit";
    } else {
        tmpIdNotification = "#notificationList";
    }
    var notification = $(tmpIdNotification).data("kendoNotification");
    notification.show({
        title: title,
        message: tmpMessage,
    }, statusNotification);
}