/*jslint browser: true, devel: true */
/*global jquery, $, window */

function setWidthAndHeight() {
    "use strict";
    var height = window.innerHeight + 15;
    var width = window.innerWidth;
    $(".sessionTracker").css("height", height).css("width", width);
}

var sm = (function ($) {
    "use strict";

    var cls = {};

    cls.touchSession = function () {
        cls.showDialogAt = 300000; // Show dialog at 5 minutes remaining in session
        
            cls.extendInUse = true;
            $.ajax({
                url: "/Account/TouchSession",
                cache: false,
                dataType: "json",
                success: cls.processResult,
                error: cls.endSession
            });
        
    };

    cls.checkRemainder = function () {
        if (!cls.extendInUse) {
            cls.extendInUse = true;
            
            $.ajax({
                url: "/Account/CheckRemainder",
                cache: false,
                dataType: "json",
                success: cls.processResult,
                error: cls.endSession
            });
        }
    };

    cls.printinfo = function () {
        var diff = cls.calcDiff();

        console.log(cls.formatRemainder(diff));
    };


    cls.hideAll = function () {
        $("#sessiontrackerOk").hide();
        $("#sessionTrackerExpired").hide();
        $("#sessionTrackerText").show();
        $("#sessiontrackerLogout").show();
        $("#sessiontrackerContinue").show();
    };

    cls.showContinue = function () {
        cls.hideAll();
        $("#sessionTrackerText").show();
        $("#sessiontrackerLogout").show();
        $("#sessiontrackerContinue").show();
    };

    cls.showMessage = function (msg) {
        cls.hideAll();
        console.log(msg);
    };


    cls.calcDiff = function () {
        return (cls.sessionEnd) - Date.now();
    };

    cls.formatRemainder = function (diff) {
        var mins = Math.floor(diff / (60000));
        diff = diff - (mins * 60000);
        var seconds = Math.floor(diff / (1000));
        diff = diff - (seconds * 1000);

        if (mins < 10) {
            mins = "0" + mins;
        }

        if (seconds < 10) {
            seconds = "0" + seconds;
        }

        var ret = "" + mins + ":" + seconds;
        //console.log(ret);

        return ret;

    };

    cls.timeBeingShown = "";
    cls.countdown = function () {
        var diff = cls.calcDiff();
        var formatted = cls.formatRemainder(diff);

        if (cls.timeBeingShown === formatted)
        {
            return;
        }
        // Check time difference between current time and when SessionManager was started plus the length of the session (minus 30 seconds)
        
        if (diff <= 0) {
            //Session has expired
            $("#sessionTrackerText").hide();
            $("#sessiontrackerLogout").hide();
            $("#sessiontrackerContinue").hide();
            $("#sessiontrackerOk").show();
            $("#sessionTrackerExpired").show();
            if (!cls.expiredCalled) {
                $.getJSON("/Account/Expire");
                cls.expiredCalled = true;
            }

            return;
        }
        if (diff <= 10000) {
            $("#sessionTimeoutCountdown").addClass("text-danger");
        } else {
            $("#sessionTimeoutCountdown").removeClass("text-danger");
        }
        // Get countdown minutes and seconds

        // Update the countdown display
        
        var current = $("#sessionTimeoutCountdown").html();
        if (current !== formatted) {
            $("#sessionTimeoutCountdown").html(formatted);
            window.setTimeout(function () { sm.countdown(); }, 1000);
        }

        // Decrement the counter
    };

    // promptToExtendSession: Display the jQuery dialog
    // and kick off the countdown timer
    cls.promptToExtendSession = function () {
        cls.countdown();
        setWidthAndHeight();
        $(".sessionTracker").fadeIn(400, function () {
            $("a[href], area[href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), iframe, object, embed, *[tabindex], *[contenteditable]").attr("tabindex", -1);
            $("#sessiontrackerLogout").attr("tabindex", 1);
            $("#sessiontrackerContinue").attr("tabindex", 1).focus();
        });
    };

    cls.processResult = function (result) {
        if (result.IsSuccess) {
            cls.MillisecondsRemaining = result.MillisecondsRemaining - 1000;  //take away an extra second to sweep the network delay problem
            cls.sessionEnd = Date.now() + cls.MillisecondsRemaining;

            if (cls.syncTimesTimeout) {
                window.clearTimeout(cls.syncTimesTimeout);
            }

            var thirtySecondsInMillis = 30000;
            if (cls.MillisecondsRemaining > thirtySecondsInMillis) {
                cls.syncTimesTimeout = window.setTimeout(cls.checkRemainder, thirtySecondsInMillis);
            }



            if (cls.promptTimeout) {
                window.clearTimeout(cls.promptTimeout);
            }

            cls.promptTimeout = window.setTimeout(cls.promptToExtendSession, (cls.MillisecondsRemaining - cls.showDialogAt));


            cls.hideAll();
            cls.extendInUse = false;

        } else {
            cls.extendInUse = false;
            cls.endSession();
        }
    };


    // endSession: Called when the session expires
    // PRC DO is cls needed to end session? Or wait for user?
    cls.endSession = function () {
        cls.extendInUse = false;
        window.location.href = "/Account/Login?ReturnUrl=" + window.location.pathname + window.location.search;
    };


    return cls;
}(jQuery));


$().ready(function () {
    "use strict";
    sm.touchSession();

    $(".sessiontracker").modal({
        backdrop: "static",
        keyboard: false
    });

    $("#sessiontrackerContinue").click(function () {
        sm.touchSession();
        $(".sessionTracker").fadeOut(400);
    });

    $("#sessiontrackerOk").click(function () {
        window.location.href = "/Account/LogOff";
    });

    $("#sessiontrackerLogout").click(function () {
        window.location.href = "/Account/LogOff";
    });

    $(window).resize(function () {
        setWidthAndHeight();
    });
});

