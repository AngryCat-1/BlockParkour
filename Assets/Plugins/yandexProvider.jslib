mergeInto(LibraryManager.library,
{
	AuthorizationCheck: function (playerPhotoSize)
	{
		AuthorizationCheck(Pointer_stringify(playerPhotoSize));
	},
	
	OpenAuthDialog: function (playerPhotoSize)
	{
		OpenAuthDialog(Pointer_stringify(playerPhotoSize));
	},
	
	SaveYG: function (jsonData, flush)
	{
		SaveCloud(Pointer_stringify(jsonData), flush);
	},
	
	LoadYG: function ()
	{
		LoadCloud();
	},
	
	InitLeaderboard: function ()
	{
		InitLeaderboard();
	},
	
	SetLeaderboardScores: function (nameLB, score)
	{
		SetLeaderboardScores(Pointer_stringify(nameLB), score);
	},
	
	GetLeaderboardScores: function (nameLB, maxPlayers, quantityTop, quantityAround, photoSizeLB)
	{
		GetLeaderboardScores(Pointer_stringify(nameLB), maxPlayers, quantityTop, quantityAround, Pointer_stringify(photoSizeLB));
	},

	FullscreenShow: function ()
	{
		FullscreenShow();
	},

    RewardedShow: function (id)
	{
		RewardedShow(id);
	},
	
	LanguageRequest: function ()
	{
		LanguageRequest();
	},
	
	RequestingEnvironmentData: function()
	{
		RequestingEnvironmentData();
	},	

	Review: function()
	{
		Review();
	}	
});