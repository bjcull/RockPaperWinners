var Game = function () {

    this.currentGameId = 0;
    this.myUserId = 0;
    this.actionSelected = false;

    init = function () {
        var self = this;

        $(".actionbutton").click(function () {
            self.submitAction($(this).data("action"), this);
        });
        
    };

    findGame = function () {
        var self = this;

        this.actionSelected = false;
        $(".actionbutton").css("display", "none").css("visibility", "visible").fadeIn("fast");

        $("#controls").fadeOut("fast");
        $("#waiting").fadeIn("fast");

        $.getJSON("/game/findgame", null, function (result) {
            var test = result;
            var opponent = {
                name: result.OpponentName,
                imagesrc: result.OpponentImageSource
            };

            self.currentGameId = result.GameResultID;
            self.myUserId = result.MyUserID;

            console.log("GameID: " + self.currentGameId + " - PlayerID: " + self.myUserId);

            $("#OpponentTemplate").tmpl(opponent).replaceAll("#opponent");
            $("#waiting").fadeOut("fast");
            $("#controls").fadeIn("fast");
        });
    };

    submitAction = function (action, button) {        
        var self = this;

        if (this.actionSelected)
            return;

        this.actionSelected = true;

        $(".actionbutton").not(button).fadeOut("fast", function () {
            $(this).css("visibility", "hidden").css("display", "inline-block");
        });

        var data = {
            gameResultID: this.currentGameId,
            playerID: this.myUserId,
            gameActionString: action
        };        


        $.getJSON("/game/submitaction", data, function (result) {

            console.log(result);

            if (result.GameResult == 1)
                $("#win").fadeIn("fast");
            else if (result.GameResult == 2)
                $("#lose").fadeIn("fast");
            else
                $("#draw").fadeIn("fast");

            setTimeout(function () {
                $(".result").fadeOut("fast");

                self.findGame();
            }, 3000);
        });
    }

    return {
        init: init,
        findGame: findGame,
        submitAction: submitAction
    }    
};