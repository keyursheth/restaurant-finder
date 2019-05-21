$(document).ready(function () {
    $('#selCity').on('change', function () {
        getRestaurantDataByCityId($(this).val(), false, '');
    });    
});

var getRestaurantDataByCityId = function (cityId, isFilterApplied, cuisines) {
    $.get('/Restaurants/GetRestaurantComponent', { entityId: cityId, isFilterApplied: isFilterApplied, cuisines: cuisines }, function (data) {
        $('#dvRestaurantList').html(data);
        $('#spCityName').text($('#selCity :selected').text() + ' Restaurants');
    });    
}

