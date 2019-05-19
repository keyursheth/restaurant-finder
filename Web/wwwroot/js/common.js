$(document).ready(function () {
    $('#selCity').on('change', function () {
        getRestaurantDataByCityId($(this).val());
    });
});

var getRestaurantDataByCityId = function (cityId) {
    $.get('/Restaurants/GetRestaurantComponent', { entityId: cityId }, function(data) {
        $('#dvRestaurantList').html(data);
        $('#spCityName').text($('#selCity :selected').text() + ' Restaurants');
    });    
}

