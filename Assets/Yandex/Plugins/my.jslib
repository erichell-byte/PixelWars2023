mergeInto(LibraryManager.library, {

  	Hello: function () {
    	window.alert("Hello, world!");
    	console.log("Hello, world!");
  	},

	GiveMePlayerData: function () {
    	myGameInstance.SendMessage('Yandex', 'SetName', player.getName());
    	myGameInstance.SendMessage('Yandex', 'SetPhoto', player.getPhoto("medium"));
    	
    	},

    Auth: function()
    {
      initPlayer().then(_player => {
        if (_player.getMode() === 'lite') {
            // Игрок не авторизован.
            ysdk.auth.openAuthDialog().then(() => {
                    // Игрок успешно авторизован
                    LoadExtern();
                    GiveMePlayerData();
                    initPlayer().catch(err => {
                        // Ошибка при инициализации объекта Player.
                      });
                  }).catch(() => {
                    // Игрок не авторизован.
                  });
                }
              }).catch(err => {
        // Ошибка при инициализации объекта Player.
      });
            },

    RateGame: function () {
    	ysdk.feedback.canReview()
                .then(({ value, reason }) => {
                    if (value) {
                        ysdk.feedback.requestReview()
                            .then(({ feedbackSent }) => {
                                console.log(feedbackSent);
                                myGameInstance.SendMessage('Yandex', 'GameWasRated');
                            })
                    } else {
                        myGameInstance.SendMessage('Yandex', 'GameWasRated');
                    }
                })
  	},

    SaveExtern: function(date) {
        var dateString = UTF8ToString(date);
        var myObj = JSON.parse(dateString);
        player.setData(myObj);
        console.log("Game has been saved" + myObj);
    },

    // LoadExtern: function() {
    //     player.getData().then(_date =>{
    //         const myJSON = JSON.stringify(_date);
    //         myGameInstance.SendMessage('Game', 'SetPlayerInfo', myJSON);
    //         console.log(myJSON);
    //         console.log("Game has been loaded");
    //     });
        
    // },

    ShowYandexInterstitialAdv: function() {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function(wasShown) {
                    console.log("----------adv showed----------------")
                    myGameInstance.SendMessage("Game", "InterstitialShowed");
                },
                onError: function(error) {
                    // some action on error
                }
                        }
                    })
    },

    ShowYandexRewarded: function(value) {
        ysdk.adv.showRewardedVideo({
        callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
        },
        onRewarded: () => {
          console.log('Rewarded!');
          myGameInstance.SendMessage("Game", "AddCoinForYandexRewarded", value);
        },
        onClose: () => {
          console.log('Video ad closed.');
        }, 
        onError: (e) => {
          console.log('Error while open video ad:', e);
        }
        }
        })
    },

    GetLang: function()
    {
        var lang = ysdk.environment.i18n.lang;
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang,buffer,bufferSize);
        return buffer;
    },

    

  });