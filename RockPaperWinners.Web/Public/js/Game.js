var Game = function () {

    init = function () {
    };

    findGame = function () {
        $.getJSON("/game/findgame", null, function (result) {
            var test = result;
            var opponent = {
                name: result.OpponentName,
                imagesrc: result.OpponentImageSource
            };

            $("#OpponentTemplate").tmpl(opponent).replaceAll("#opponent");
        });
    };

    submitAction = function (action) {
        $.getJSON("/game/submitaction", null, function (result) {
        });
    }

    return {
        init: init,
        findGame: findGame
    }    
};