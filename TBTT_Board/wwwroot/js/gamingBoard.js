$(function () {

    var treeCourt1 = '';
    var is_moving = false;
    var elements = [];
    var elementsID = [];
    var elementsOrder = [];
    var elementsType = [];
    var courtName = 'Court-1';
    var divCourt1 = '#divCourt1';
    var divFooter = '#divCourt1Footer';
    var elapsedMinutesConstant = Number(TBTTCONSTANT.ELAPSED_MINUTES_CONSTANT);
    var intervalStartCourtTime;
    var touchTimer = 0;

    function loadCourtGames(courtName, divCourtID) {
        var reqUrl = $("div#urlGetGameList").data('request-url');
        var divFooterData = $(divFooter).text();
        treeCourt1 = $(divCourtID);

        var courtRootName = courtName;
        var prefix = '<ul><li id="CourtNameRoot" data-nodeid = "-999" data-jstree=\'{\"icon\":\"fa fa-hourglass-1\"}\'>' + '<a class="courtName" data-nodeid = "-999" href="#"> ' + courtRootName + ' </a>';
        var suffix = "</li></ul>";

        //ajax call to render group tree
        //in complete function call jstree initializer on Groups-Tree div
        //"contextmenu", 
        $.ajax({
            type: "GET",
            contentType: 'application/json',
            url: reqUrl,
            async: false,
            data: { CourtName: courtName },
            cache: false,
            success: function (html) {
                if (html.length <= 40) {
                    $(divFooter).html(''); //$(divFooter).html('');
                }

                if (divFooterData != null) {
                    divFooterData = divFooterData.trim();
                }
                treeCourt1.jstree('destroy');
                treeCourt1.html('');
                html = prefix + html + suffix;
                //html =  html;
                treeCourt1.html(html);
                treeCourt1.jstree({
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
                    "plugins": ["dnd", "contextmenu"], //, "wholerow", "state", "types", "search"
                    'contextmenu': {
                        'items': customDeleteCourtMember
                    },
                    "dnd": {
                        is_draggable: true
                    }
                });

                treeCourt1.jstree("open_all");
                treeCourt1.jstree(true).redraw();
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




        return;
    }


    function customDeleteCourtMember(node) {
        var items = {
            deleteItem: {
                label: "Delete",
                action: function () {
                    var reqUrl = $("div#urlDeleteFromGameBoardByMembershipID").data('request-url');
                    var token = $('[name=__RequestVerificationToken]').val();
                    var headers = {};
                    headers["__RequestVerificationToken"] = token;
                    //var ref = $(divCourt1).jstree(true);
                    //var playTree = $('#divPlayingList').jstree(true);
                    var current;

                    if (node != null) {
                        if (node.a_attr != null) {
                            current = node.a_attr["data-nodemembername"];
                            courtName = node.a_attr["data-nodecourt"];
                        }
                    }

                    if (current!=null) {

                        var duplicate = false;
                        $('div#divPlayingList' + ' li').each(function () {
                            if (current == $(this).children('a').text()) {
                                duplicate = true;
                            }
                        });

                        if (duplicate == false) {

                            $.ajax({
                                type: "POST",
                                contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                                headers: headers,
                                url: reqUrl,
                                data: { MemberName: current, CourtName: courtName },
                                cache: false,
                                success: function (html) {
                                    var htmlOut = html;
                                    var htmlMsg = '';
                                    if (htmlOut != null) {
                                        htmlMsg = htmlOut.responseText
                                    }

                                    if (htmlMsg == TBTTCONSTANT.SUCCESS_MESSAGE) {
                                        loadCourtGames(courtName, divCourt1);
                                        loadPlayingList();

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

                            var divFooterTime = $(divFooter).html();

                            if (divFooterTime != null) {
                                divFooterTime = divFooterTime.replace(/^\s+|\s+$/g, '');
                            }
                            else {
                                divFooterTime = '';
                            }

                            if ((divFooterTime == '') || (divFooterTime == '00:00')) {
                                $(divFooter).html('00:00');
                                 intervalStartCourtTime = setInterval(function () {
                                    startCourtTime(courtName, divCourt1, divFooter);
                                    clearInterval(intervalStartCourtTime);

                                }, 1000);
                            }

                        }

                    }
                    //redraw and expand tree
                    treeCourt1.jstree(true).open_all();
                    treeCourt1.jstree(true).redraw();
                  
                },
            }
        }

        return items;
    }


    //PLAYING LIST LOADS END HERE

    $('div#divPlayingList').on('mousedown touchstart', 'a.jstree-anchor', function (e) {

        is_moving = true;
        elements = [];
        elementsID = [];
        elementsOrder = [];
        elementsType = [];
        var ref = $('#divPlayingList').jstree(true);
        var current = $(this).closest('li');
        var nodeID = current.attr('id');
        while (current != null) {

            //elementsID.push(current.children('a').data('nodeid'));
            //elementsOrder.push(current.children('a').data('nodeOrderID'));
            nodeID = ref.get_node(current);
            if (nodeID != null) {
                if (nodeID.data != null) {
                    if (nodeID.data.nodeid != -999) {
                        elements.push(current.children('a').text());
                        elementsID.push(nodeID.data.nodeid);
                        elementsOrder.push(nodeID.data.nodeorderid);
                        elementsType.push(nodeID.data.nodemembershiptype);
                    }
                }
            }
            nodeID = ref.get_parent(nodeID);
            try {
                if (nodeID == '#') {
                    break;
                }
                current = $('#' + nodeID);
            }
            catch (ex) {
            }

        }
        return;
    });
    
    
    $(divCourt1).on("touchend", "a.jstree-anchor", function () {
        if (touchTimer == 0) {
            touchTimer = 1;
            touchTimer = setTimeout(function () { touchTimer = 0; }, 600);
        }
        else {
            //Execute below code on double touch.
            var reqUrl = $("div#urlDeleteFromGameBoardByMembershipID").data('request-url');
            var token = $('[name=__RequestVerificationToken]').val();
            var headers = {};
            headers["__RequestVerificationToken"] = token;
            var ref = $(divCourt1).jstree(true);
            var playTree = $('#divPlayingList').jstree(true);
            //set parent to root element
            var parentID = 'CourtNameRoot';
            var i = 0;
            var length = i;


            is_moving = true;
            elements = [];
            elementsID = [];
            elementsOrder = [];
            elementsType = [];
            var ref = $(divCourt1).jstree(true);
            var current = $(this).closest('li');
            var nodeID = current.attr('id');
            while (current != null) {
                nodeID = ref.get_node(current);
                if (nodeID != null) {
                    if (nodeID.data != null) {
                        if (nodeID.data.nodeid != -999) {
                            elements.push(current.children('a').text());
                        }
                    }
                }
                nodeID = ref.get_parent(nodeID);
                try {
                    if (nodeID == '#') {
                        break;
                    }
                    current = $('#' + nodeID);
                }
                catch (ex) {
                }

            }

            i = elements.length - 1;
            length = i;

            while (i >= 0) {
                var current = elements.pop();
                var currentID = elementsID.pop();
                var currentOrder = elementsOrder.pop();
                var currentType = elementsType.pop();
                var duplicate = false;
                $('div#divPlayingList' + ' li').each(function () {
                    if (current == $(this).children('a').text()) {
                        duplicate = true;
                    }
                });
                if (duplicate == false) {

                    $.ajax({
                        type: "POST",
                        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                        headers: headers,
                        url: reqUrl,
                        data: { MemberName: current, CourtName: courtName },
                        cache: false,
                        success: function (html) {
                            var htmlOut = html;
                            var htmlMsg = '';
                            if (htmlOut != null) {
                                htmlMsg = htmlOut.responseText
                            }

                            if (htmlMsg == TBTTCONSTANT.SUCCESS_MESSAGE) {
                                loadCourtGames(courtName, divCourt1);
                                loadPlayingList();

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

                    var divFooterTime = $(divFooter).html();

                    if (divFooterTime != null) {
                        divFooterTime = divFooterTime.replace(/^\s+|\s+$/g, '');
                    }
                    else {
                        divFooterTime = '';
                    }

                    if ((divFooterTime == '') || (divFooterTime == '00:00')) {
                        $(divFooter).html('00:00');
                         intervalStartCourtTime = setInterval(function () {
                            startCourtTime(courtName, divCourt1, divFooter);
                            clearInterval(intervalStartCourtTime);

                        }, 1000);
                    }

                }
                i--;
            }

            //redraw and expand tree
            treeCourt1.jstree(true).open_all();
            treeCourt1.jstree(true).redraw();

            is_moving = false;
            elements = [];
            elementsID = [];
            elementsType = [];
        }
       
        return;
    });




    $(divCourt1).on('mouseup touchend', 'a.jstree-anchor', function (e) {
        var recordAdded = false;
        is_moving = true;
        if (is_moving == true) {

            var reqUrl = $("div#urlSaveGameBoard").data('request-url');
            var token = $('[name=__RequestVerificationToken]').val();
            var headers = {};
            headers["__RequestVerificationToken"] = token;
            var ref = $(divCourt1).jstree(true);
            var playTree = $('#divPlayingList').jstree(true);
            //set parent to root element
            var parentID = 'CourtNameRoot';
            var i = elements.length - 1;
            var length = i;
            //loop through all the moved elements starting at first parent
            //builds the tree
            while (i >= 0) {
                var current = elements.pop();
                var currentID = elementsID.pop();
                var currentOrder = elementsOrder.pop();
                var currentType = elementsType.pop();
                var duplicate = false;
                $(divCourt1 + ' li').each(function () {
                    if (current == $(this).children('a').text()) {
                        //parentID = $(this).attr('id');
                        duplicate = true;
                    }
                });
                if ((currentID == -999) || (currentID == null)) {
                    duplicate = true;
                }
                if (duplicate == false) {

                    $.ajax({
                        type: "POST",
                        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                        headers: headers,
                        url: reqUrl,
                        data: { MembershipID: currentID, MemberName: current, CourtName: courtName, OrderID: currentOrder, MembershipType: currentType },
                        cache: false,
                        success: function (html) {
                            var htmlOut = html;
                            var htmlMsg = '';
                            if (htmlOut != null) {
                                htmlMsg = htmlOut.responseText
                            }

                            if (htmlMsg == TBTTCONSTANT.SUCCESS_MESSAGE) {
                                loadCourtGames(courtName, divCourt1);
                                loadPlayingList();
                                 var currentNode = '';

                                //var currentNode = playTree.get_node(current);

                                //if (currentNode != null) {
                                //    playTree.delete_node(currentNode, true);
                                //}
                                var anodeID;

                                //$('#divPlayingList li').each(function () {
                                //     anodeID = $(this).children('a').data('nodeid');
                                //    if ((currentID == $(this).data('nodeid')) || (currentID == anodeID)) {
                                //        currentNode = $(this);
                                //        playTree.delete_node(currentNode, true);
                                //    }
                                //});
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

                    var divFooterTime = $(divFooter).html();

                    if (divFooterTime != null) {
                        divFooterTime = divFooterTime.replace(/^\s+|\s+$/g, '');
                    }
                    else {
                        divFooterTime = '';
                    }

                    if ((divFooterTime == '') || (divFooterTime == '00:00')) {
                        $(divFooter).html('00:00');
                         intervalStartCourtTime = setInterval(function () {
                            startCourtTime(courtName, divCourt1, divFooter);
                            clearInterval(intervalStartCourtTime);

                        }, 1000);
                    }  

                }
                i--;
            }
            //try {
            //    loadCourtGames(courtName, divCourt1);
            //    loadPlayingList();
            //}
            //catch (ex) {
            //}
            //redraw and expand tree
            treeCourt1.jstree(true).open_all();
            treeCourt1.jstree(true).redraw();


        }
        
        

        is_moving = false;
        elements = [];
        elementsID = [];
        elementsType = [];
        return;
    });

    
    function updateStartTime(paramCourtName, footcourtID, divTree) {
         var elapsedTimeFormated = $(footcourtID).text();

        if (elapsedTimeFormated == '') {
            return;
        }
        if ($(footcourtID) == null) {
            return;
        }

        var myTime = elapsedTimeFormated;
        var ss;

        if (elapsedTimeFormated != null) {
            if (elapsedTimeFormated.length > 0) {
                if (elapsedTimeFormated.toString().indexOf(':') != -1) {
                    ss = myTime.split(':');
                }
                else if (elapsedTimeFormated.toString().indexOf('.') != -1) {
                    ss = myTime.split('.');
                }
            }
        }
        

        var dt = new Date();
        var newElapsedTimeFormated = '00:00';
        var hourPassed = false;
        dt.setHours(0);
        dt.setMinutes(0);
        dt.setSeconds(0);
        if (ss != null) {
            if (ss.length > 1) {
                dt.setHours(0);
                if (ss[1] >= 60) {
                    ss[0] = parseInt(ss[0]) + parseInt(ss[1]) / 60;
                    ss[1] = parseInt(ss[1]) % 60;
                    dt.setSeconds(parseInt(ss[1]));
                }
                else {
                    dt.setSeconds(ss[1]);
                }
                if (ss[0] >= 60) {
                    dt.setHours(1);
                    dt.setMinutes(Math.round((parseFloat(ss[0]) % 60)));
                    hourPassed = true;
                }
                else {
                    dt.setMinutes(parseInt(ss[0]));
                }                
            }
            else {
                    dt.setHours(0);
                    dt.setMinutes(0);
                    dt.setSeconds(0);
            }
        }

        var dt2 = new Date(dt.valueOf() + 1000);


        var temp = dt2.toTimeString().split(" ");
        var ts = temp[0].split(':');

        if (ts != null) {
            if (ts.length > 1) {
                newElapsedTimeFormated = ts[1] + ":" + ts[2];
                timecheck = ts[1];
            }
            else if (ts.length > 0) {
                newElapsedTimeFormated = ts[1] + ":" + '0';
                timecheck = ts[1];
            }
        }
        
        $(footcourtID).html(newElapsedTimeFormated);

        var elapsedMinutes = 0;
        var isReadyToLoad = false;
        if (newElapsedTimeFormated.toString().indexOf(':') != -1) {
            elapsedTimeArray = '';
            elapsedTimeArray = newElapsedTimeFormated.toString().split(':');

            if (elapsedTimeArray != null) {
                if (elapsedTimeArray.length > 0) {
                    elapsedMinutes = elapsedTimeArray[0];
                    elapsedSeconds = elapsedTimeArray[1];

                    if (elapsedMinutes >= 15) {
                        $(footcourtID).css('color', 'Red');
                        $(footcourtID).css('font-weight', 'bold');
                        $(footcourtID).css('font-size', '20px');

                        //font-weight: bold;
                    }
                    else {
                        $(footcourtID).css('color', 'green');
                        $(footcourtID).css('font-weight', 'bold');
                        $(footcourtID).css('font-size', '20px');
                    }
                    if ((elapsedMinutes >= elapsedMinutesConstant) || (hourPassed)) { 
                        $(footcourtID).html('');
                    }
                }
            }
        }


        return;
    }


    function startCourtTime(courtName, divTree, divFooter) {
    var current = '';
    var gameElapsedTime = 0;
    var nodeID = '';
    var canStartStopWatch = false;
    var counter = 0;
    var dt = new Date();
    var elapsedTimeFormated = '';
    var nodeCount = 0;
    var isPlayListReadyToRefresh = false;


    $(divTree + ' li').each(function () {
        counter = counter + 1;
        current = $(this);
        nodeID = current.children('a').data('nodeid');
        if (nodeID != -999) {
            child = current.children('a');
            gameElapsedTime = current.children('a').data('gameelapsedtime');
            nodeCount = current.children('a').data('nodecount');
            if (gameElapsedTime != null) {
                canStartStopWatch = true;
            }
        }
    });

    if (canStartStopWatch) {

        var elapsedTimeArray;

        if (gameElapsedTime.toString().indexOf('.') != -1) {
            elapsedTimeArray = gameElapsedTime.toString().split('.');
        }

        if (gameElapsedTime.toString().indexOf(':') != -1) {
            elapsedTimeArray = '';
            elapsedTimeArray = gameElapsedTime.toString().split(':');
        }

        if (elapsedTimeArray != null) {
            if (elapsedTimeArray.length > 1) {
                elapsedTimeFormated = elapsedTimeArray[0] + ':' + elapsedTimeArray[1];
            }
            else if (elapsedTimeArray.length > 0) {
                elapsedTimeFormated = elapsedTimeArray[0];
            }
        }
        else {
            elapsedTimeFormated = gameElapsedTime.toString() + ':' + '00';
        }

        $(divFooter).html(elapsedTimeFormated);
    }


        if (((elapsedTimeFormated != '') && (elapsedTimeFormated != '00:00')) || (nodeCount > 0)) {

        var intervalStartCourtTimeUpdated = setInterval(function () {       
            elapsedTimeFormated = $(divFooter).text();
            if (elapsedTimeFormated == '') {   //Reset of Counter happens here.      
                isPlayListReadyToRefresh = true;                
                loadCourtGames(courtName, divCourt1);
                loadPlayingList();     
                //Check if there are more members from queue got added.
                var counter = 0;
                $(divCourt1 + ' li').each(function () {
                    current = $(this);
                    nodeID = current.children('a').data('nodeid');
                    if (nodeID != -999) {
                        counter = counter + 1;
                    }
                });
                if (counter >= 1) {
                    $(divFooter).html('00:01'); //Just keep updated on the timing for refresh page.
                    updateStartTime(courtName, divFooter, divTree);
                }
                else {
                    clearInterval(intervalStartCourtTimeUpdated);
                }

            }
            else {               
                updateStartTime(courtName, divFooter, divTree);
            }
        }, 1000); //This is where time counter loop works.
        }

    return;
    }


    

 loadCourtGames(courtName, divCourt1);
       

 intervalStartCourtTime = setInterval(function () {
    var counter = 0;
    
    $(divCourt1 + ' li').each(function () {
        current = $(this);
        nodeID = current.children('a').data('nodeid');
        if (nodeID != -999) {
            counter = counter + 1;
        }
    });



    if (counter >= 1) {
        startCourtTime(courtName, divCourt1, divFooter);
        clearInterval(intervalStartCourtTime);
    }
    else {
         clearInterval(intervalStartCourtTime);
    }

}, 1000); //Works Only on Page Refresh. Counter never continues.

    

}); 