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

            if (!result) {
                findGame();
                return;
            }            

            var opponent = {
                name: result.OpponentName,
                imagesrc: result.OpponentImageSource
            };

            self.currentGameId = result.GameResultID;
            self.myUserId = result.MyUserID;

            console.log("GameID: " + self.currentGameId + " - PlayerID: " + self.myUserId);

            $("#OpponentTemplate").tmpl(opponent).replaceAll("#opponent-wrapper");
            $("#waiting").fadeOut("fast");
            $("#controls").fadeIn("fast");
        });
    };

    submitAction = function (action, button) {        
        var self = this;

        if (this.actionSelected)
            return;

        this.actionSelected = true;

        $("#thinking").fadeIn("fast");

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

            if (result.GameResult == 1) {
                $("#win").fadeIn("fast");

                if (action == "Rock")
                    $("#Oscissors").fadeIn("fast");
                else if (action == "Paper")
                    $("#Orock").fadeIn("fast");
                else if (action == "Scissors")
                    $("#Opaper").fadeIn("fast");
            }
            else if (result.GameResult == 2) {
                $("#lose").fadeIn("fast");

                if (action == "Rock")
                    $("#Opaper").fadeIn("fast");
                else if (action == "Paper")
                    $("#Oscissors").fadeIn("fast");
                else if (action == "Scissors")
                    $("#Orock").fadeIn("fast");
            }
            else {
                $("#draw").fadeIn("fast");

                if (action == "Rock")
                    $("#Orock").fadeIn("fast");
                else if (action == "Paper")
                    $("#Opaper").fadeIn("fast");
                else if (action == "Scissors")
                    $("#Oscissors").fadeIn("fast");
            }

            $("#thinking").fadeOut("fast");

            setTimeout(function () {
                $(".result").fadeOut("fast");
                $("opponent-action").fadeOut("fast");

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