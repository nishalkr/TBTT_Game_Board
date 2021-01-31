//function touchHandler(event) {
//    var touches = event.changedTouches,
//        first = touches[0],
//        type = "";
//    switch (event.type) {
//        case "touchstart": type = "mousedown"; break;
//        case "touchmove": type = "mousemove"; break;
//        case "touchend": type = "mouseup"; break;
//        default: return;
//    }

//    var simulatedEvent = document.createEvent("MouseEvent");
//    simulatedEvent.initMouseEvent(type, true, true, window, 1,
//        first.screenX, first.screenY,
//        first.clientX, first.clientY, false,
//        false, false, false, 0/*left*/, null);
//    first.target.dispatchEvent(simulatedEvent);
//    return;
//   //event.preventDefault();
//}

//function init() {
//    document.addEventListener("touchstart", touchHandler, false);
//    document.addEventListener("touchmove", touchHandler, false);
//    document.addEventListener("touchend", touchHandler, false);
//    document.addEventListener("touchcancel", touchHandler, false);
//    return;
//}


var treePlayingList = '';
var treeAvailableList = '';
var is_moving = false;
var elements = [];
var elementsID = [];
var elementsType = [];

function customMenuGameBoard(node) {
    var items = {
        deleteItem: {
            label: "Delete",
            action: function () { },
        }
    }

    return items;
}




function customMenuPlayingList(node) {
    var items = {
        deleteItem: {
            label: "Toggle Freeze",
            action: function () {
                var token = $('[name=__RequestVerificationToken]').val();
                var headers = {};
                headers["__RequestVerificationToken"] = token;
                var reqUrl = $("div#urlSaveWaitingListNodeStatus").data('request-url');
                var nodeStatus;
                var currentID;

                if (node != null) {
                    if (node.data != null) {
                        if (node.data.nodestatus != null) {
                            nodeStatus = node.data.nodestatus;
                        }
                        if (node.data.nodeid != null) {
                            currentID = node.data.nodeid;
                        }
                    }
                }

                if (nodeStatus != null) {
                    if (nodeStatus == 'True') {
                        nodeStatus = 'False';
                    }
                    else {
                        nodeStatus = 'True';
                    }
                }
                $.ajax({
                    type: "POST",
                    contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                    headers: headers,
                    url: reqUrl,
                    data: { MembershipID: currentID, NodeStatus: nodeStatus },
                    cache: false,
                    success: function (html) {
                        var htmlOut = html;
                        var htmlMsg = '';
                        if (htmlOut != null) {
                            htmlMsg = htmlOut.responseText
                        }

                        if (htmlMsg == TBTTCONSTANT.SUCCESS_MESSAGE) {
                            var ref = $('#divPlayingList').jstree(true);

                            if (nodeStatus == 'False') {
                                if (node != null) {
                                    ref.disable_node(node);
                                    if (node.data != null) {
                                        if (node.data.nodestatus != null) {
                                            node.data.nodestatus = 'False';
                                        }
                                    }
                                }
                            }
                            else {
                                if (node != null) {
                                    ref.enable_node(node);
                                    if (node.data != null) {
                                        if (node.data.nodestatus != null) {
                                            node.data.nodestatus = 'True';
                                        }
                                    }
                                }
                            }

                            success(TBTTCONSTANT.SUCCESS_MESSAGE);
                        }
                        if (html == TBTTCONSTANT.GENERIC_ERROR) {
                            error(TBTTCONSTANT.GENERIC_ERROR);
                        }
                    },
                    error: function (xhr) { //Handle session expiration.
                        if (xhr.status === 401) {
                            window.location.href = xhr.Data.LogOnUrl;
                            return;
                        }
                        if (xhr.status === 403) {
                            error(TBTTCONSTANT.UNAUTHORIZEDREQUEST);
                        }
                        else {
                            error(TBTTCONSTANT.GENERIC_ERROR);
                        }
                    }
                });
            },
        }
    }
    loadPlayingList();
    return items;
}


function activatePlayingcList() {
    var treeAvailableList = $("div#divAvailableList");
    treeAvailableList.jstree({
        "core": {
            "multiple": false,
            'check_callback': function (operation, node, node_parent, node_position, more) {
                if (operation === "delete_node") {
                    return true;
                }
                if (operation === "create_node") {
                    return false;
                }
                if (operation === "move_node") {
                    return false;
                }
                return true;  //allow other operations
            }
        },
        "plugins": ["dnd", "search", "state", "types"], //, "wholerow"
        "dnd": {
            check_while_dragging: true            
        }

    });
    treeAvailableList.redraw();
    treeAvailableList.jstree("open_all");
}

function setNodeStatusOnLoad() {
    var nodeID;
    var nodeStatus = 'True';
    if (treePlayingList == null) {
        treePlayingList = $('#divPlayingList').jstree(true);
    }

    $("#divPlayingList").bind('ready.jstree', function (event, data) {
        $(this).jstree().open_all(); // open all nodes so they are visible in dom
        $('#divPlayingList li').each(function () {
            var node = $("#divPlayingList").jstree().get_node(this.id);
            nodeID = node.id;
            if (nodeID != -999) {
                nodeStatus = node.data.nodestatus;
                if (nodeStatus != null) {
                    if ((nodeStatus == 'False') || (!nodeStatus)) {
                        $(this).jstree().disable_node(node);
                    }
                    else {
                        $(this).jstree().enable_node(node);
                    }
                }

            }

        });
        // $(this).jstree().close_all(); // close all again
    });

    return;
}

function loadPlayingList() {
    var reqUrl = $("div#urlGetWaitingList").data('request-url');

    treePlayingList = $("div#divPlayingList");

    var playingRootName = 'Add Players Here';
    //var prefix = '<ul><li id="playingListRoot" data-nodeid = "-999" data-jstree=\'{\"icon\":\"fa fa-cubes\"}\'>' + '<a data-nodeid = "-999" href="#"> ' + playingRootName + ' </a>';
    //var suffix = "</li></ul>";

    var objPlayingName = document.getElementById("txtPlaying");
    var playingName = '';

    if (objPlayingName != null) {
        playingName = objPlayingName.value;
    }


    //ajax call to render group tree
    //in complete function call jstree initializer on Groups-Tree div
    $.ajax({
        type: "GET",
        contentType: 'application/json',
        url: reqUrl,
        data: { PlayingName: playingName },
        cache: false,
        success: function (html) {
            treePlayingList.jstree('destroy');
            //html = prefix + html + suffix;
            //html =  html;
            treePlayingList.html(html);
            treePlayingList.jstree({
                "core": {
                    "multiple": false,
                    'check_callback': function (operation, node, node_parent, node_position, more) {
                        if (operation === "delete_node") {
                            return true;
                        }
                        if (operation === "move_node") {
                            return false;
                        }
                        return true;  //allow other operations
                    }
                },
                "plugins": ["dnd", "search", "state", "types"], //"contextmenu",, "wholerow"
                'contextmenu': {
                    'items': customMenuPlayingList
                },
                "dnd": {
                    is_draggable: true
                }
            });
            treePlayingList.jstree("open_all");
            //var form = treePlayingList.closest("form");
            //ResetUnobtrusiveValidation(form);

        },
        error: function (xhr) { //Handle session expiration.
            if (xhr.status === 401) {
                error(TBTTCONSTANT.UNAUTHORIZEDREQUEST);
                return;
            }
            if (xhr.status === 403) {
                error(TBTTCONSTANT.UNAUTHORIZEDREQUEST);
            }
            else {
                error(TBTTCONSTANT.GENERIC_ERROR);
            }
        }
    });
    //treePlayingList.jstree(true).redraw();
    //treePlayingList.jstree(true).open_all();
    setNodeStatusOnLoad();
    return;
}


function addAvailableList(formData) {
    var reqUrl = $("div#urlSaveAvailableList").data('request-url');
    var token = $('[name=__RequestVerificationToken]').val();
    var headers = {};
    headers["__RequestVerificationToken"] = token;
    var objMsg = document.getElementById("divMsg");
    objMsg.innerHTML = '';

    if (formData.length <= 3) {
        objMsg.style.display = "block";
        objMsg.innerHTML = 'Length of Member Name - ' + formData + ', should be minimum 4 characters.'
        return false;
    }

    $.ajax({
        type: "POST",
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        headers: headers,
        url: reqUrl,
        async: false,
        data: { formData: formData},
        cache: false,
        success: function (html) {
            if (html.responseText == TBTTCONSTANT.DUPLICATE_MESSAGE) {
                objMsg.style.display = "block";
                formData = formData.replace('%', '');
                formData = formData.replace('?', '');
                objMsg.innerHTML = 'Duplicate Name - ' + formData + '. Please add a unique name not in Board.'
                return false;
            }
            if (html.responseText == TBTTCONSTANT.INVALID_MEMBERSHIPID) {
                objMsg.style.display = "block";
                formData = formData.replace('%', '');
                formData = formData.replace('?', '');
                objMsg.innerHTML = 'Invalid MembershipID - ' + formData + '. Please contact front desk for any questions.'
                return false;
            }
        },
        error: function (xhr) { //Handle session expiration.
            if (xhr.status === 401) {
                error(TBTTCONSTANT.UNAUTHORIZEDREQUEST);
                return;
            }
            if (xhr.status === 403) {
                error(TBTTCONSTANT.UNAUTHORIZEDREQUEST);
            }
            else {
                error(TBTTCONSTANT.GENERIC_ERROR);
            }
        }
    });
    loadPlayingList();
    return true;
}

function gameBoardVisibility(courtName) {
    var objCourtName1 = document.getElementById("divParentCourt1");
    var objCourtName2 = document.getElementById("divParentCourt2");
    var objCourtName3 = document.getElementById("divParentCourt3");
    var objCourtName4 = document.getElementById("divParentCourt4");
    var objCourtName5 = document.getElementById("divParentCourt5");
    var objCourtName6 = document.getElementById("divParentCourt6");
    var objCourtName7 = document.getElementById("divParentCourt7");
    var objCourtName8 = document.getElementById("divParentCourt8");
    //var objCourtName9 = document.getElementById("divParentCourt9");
    var hidCourt1 = $("#Court-1").val();
    var hidCourt2 = $("#Court-2").val();
    var hidCourt3 = $("#Court-3").val();
    var hidCourt4 = $("#Court-4").val();
    var hidCourt5 = $("#Court-5").val();
    var hidCourt6 = $("#Court-6").val();
    var hidCourt7 = $("#Court-7").val();
    var hidCourt8 = $("#Court-8").val();
    //var hidCourt9 = $("#Court-9").val();

    if (courtName.length > 0) {
        objCourtName1.style.display = "none";
        objCourtName2.style.display = "none";
        objCourtName3.style.display = "none";
        objCourtName4.style.display = "none";
        objCourtName5.style.display = "none";
        objCourtName6.style.display = "none";
        objCourtName7.style.display = "none";
        objCourtName8.style.display = "none";
        //objCourtName9.style.display = "none";

        switch (courtName) {
            case '1':
                if (hidCourt1.toLowerCase() == "true") {
                    objCourtName1.style.display = "block";
                }
                break;
            case '2':
                if (hidCourt2.toLowerCase() == "true") {
                    objCourtName2.style.display = "block";
                }
                break;
            case '3':
                if (hidCourt3.toLowerCase() == "true") {
                    objCourtName3.style.display = "block";
                }
                break;
            case '4':
                if (hidCourt4.toLowerCase() == "true") {
                    objCourtName4.style.display = "block";
                }
                break;
            case '5':
                if (hidCourt5.toLowerCase() == "true") {
                    objCourtName5.style.display = "block";
                }
                break;
            case '6':
                if (hidCourt6.toLowerCase() == "true") {
                    objCourtName6.style.display = "block";
                }
                break;
            case '7':
                if (hidCourt7.toLowerCase() == "true") {
                    objCourtName7.style.display = "block";
                }
                break;
            case '8':
                if (hidCourt8.toLowerCase() == "true") {
                     objCourtName8.style.display = "block";
                }         
                break;
            default:
                objCourtName1.style.display = "block";
                objCourtName2.style.display = "block";
                objCourtName3.style.display = "block";
                objCourtName4.style.display = "block";
                objCourtName5.style.display = "block";
                objCourtName6.style.display = "block";
                objCourtName7.style.display = "block";
                objCourtName8.style.display = "block";
                objCourtName9.style.display = "block";
                loadGameBoard();
                
        }
    }
    else {
        //objCourtName1.style.display = "block";
        //objCourtName2.style.display = "block";
        //objCourtName3.style.display = "block";
        //objCourtName4.style.display = "block";
        //objCourtName5.style.display = "block";
        //objCourtName6.style.display = "block";
        //objCourtName7.style.display = "block";
        //objCourtName8.style.display = "block";
        //objCourtName9.style.display = "block";
        loadGameBoard();
    }

    return;
}

function toggleGameBoard() {

    var objGame = document.getElementById("ddlGame").value;
    var gameBoard = '';
    var objCourtName;

     gameBoardVisibility(objGame);
    //addPlayingList();
    return;
}

function displayGameBoard(gameBoard, hidCourt) {

    if (hidCourt.toLowerCase() == "false") {
        gameBoard.hide();
    } else {
        gameBoard.show();
    } 

    return;
}

function loadGameBoard() {
    var gameBoard1 = $("#divParentCourt1");
    var hidCourt1 = $("#Court-1").val();
    var gameBoard2 = $("#divParentCourt2");
    var hidCourt2 = $("#Court-2").val();
    var gameBoard3 = $("#divParentCourt3");
    var hidCourt3 = $("#Court-3").val();
    var gameBoard4 = $("#divParentCourt4");
    var hidCourt4 = $("#Court-4").val();
    var gameBoard5 = $("#divParentCourt5");
    var hidCourt5 = $("#Court-5").val();
    var gameBoard6 = $("#divParentCourt6");
    var hidCourt6 = $("#Court-6").val();
    var gameBoard7 = $("#divParentCourt7");
    var hidCourt7 = $("#Court-7").val();
    var gameBoard8 = $("#divParentCourt8");
    var hidCourt8 = $("#Court-8").val();
    //var gameBoard9 = $("#divParentCourt9");
    //var hidCourt9 = $("#Court-9").val();


    displayGameBoard(gameBoard1, hidCourt1);
    displayGameBoard(gameBoard2, hidCourt2);
    displayGameBoard(gameBoard3, hidCourt3);
    displayGameBoard(gameBoard4, hidCourt4);
    displayGameBoard(gameBoard5, hidCourt5);
    displayGameBoard(gameBoard6, hidCourt6);
    displayGameBoard(gameBoard7, hidCourt7);
    displayGameBoard(gameBoard8, hidCourt8);
    //displayGameBoard(gameBoard9, hidCourt9);
      
    return;
}


$(function () {      
    //var objMembershipID = document.getElementById("txtMembershipID");
    var objAvailable = document.getElementById("txtAvailable");
    var objPlaying = document.getElementById("txtPlaying");
    var objGame = document.getElementById("ddlGame");
    var objMsg = document.getElementById("divMsg");
    var formData = '';
    objMsg.style.display = "none";
    var errorMsg = '';

    objPlaying.addEventListener("keypress", function (event) {
        objMsg.style.display = "none";
        if (objPlaying != null) {
            formData = objPlaying.value;
        }
        var hasValues=$('li:contains("' + formData + '")').show();

        if (event.keyCode == 13) {
            if (objPlaying != null) {
                formData = objPlaying.value;
            }

            //Check if the User input is already present in the Playing List
            if (hasValues.length > 0) {
                loadPlayingList();
            }

            //Check if the user input starts with B0 or B9
            else if (!(formData.startsWith("B0")) || (formData.startsWith("B9"))) {
                objMsg.innerHTML = 'Membership ID should start with B0 or B9. Please contact front desk for any questions.'
                objMsg.style.display = "block";
                objMsg.value = '';
            }
            else if (!(formData.length >= 6)) {
                objMsg.innerHTML = 'Invalid MembershipID - ' + formData + '. Please contact front desk for any questions.'
                objMsg.style.display = "block";
                objMsg.value = '';
                return;
            }
            else {
                if (hasValues.length == 0) {
                    addAvailableList(formData);
                    location.reload();
                    false;
                }
            }
            objPlaying.value = '';
        }
        return;
    });

    //init();
    loadPlayingList();      
    loadGameBoard();
});

loadPlayingList();
function onSelectChange() {
    var x = document.getElementById("ddlGame").value;
    toggleGameBoard();
}
$.expr[":"].contains = $.expr.createPseudo(function (arg) {
    return function (elem) {
        return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
    };
});