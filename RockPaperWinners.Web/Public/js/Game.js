var Game = function () {

    this.currentGameId = 0;
    this.myUserId = 0;

    init = function () {
        var self = this;

        $(".actionbutton").click(function () {
            self.submitAction($(this).data("action"));
        });
        
    };

    findGame = function () {
        var self = this;

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

            $("#OpponentTemplate").tmpl(opponent).replaceAll("#opponent");
            $("#waiting").fadeOut("fast");
            $("#controls").fadeIn("fast");
        });
    };

    submitAction = function (action) {
        var self = this;

        var data = {
            gameResultID: this.currentGameId,
            playerID: this.myUserId,
            gameActionString: action
        };        

        $.getJSON("/game/submitaction", data, function (result) {
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