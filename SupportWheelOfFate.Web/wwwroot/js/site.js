$(document).ready(function () {
    var data = { rota : { shifts: [] } };
    new Vue({
        el: '#rota',
        data: data
    });
    
    //TODO: Remove the hardcoded url
    //TODO: Handle errors
    $.get("api/support/rota", function (rota) {
        data.rota.shifts = convertToViewModel(rota.shifts);
    })
});

//TODO: This should be unit tested
function convertToViewModel(model) {
    var result = [];
    Object.keys(model).map(function (key) {
        var entry = {};
        entry.date = new Date(key);
        entry.isToday = new Date().setHours(0,0,0,0) === entry.date.setHours(0,0,0,0);
        entry.isPast = new Date().setHours(0,0,0,0) > entry.date.setHours(0,0,0,0);
        entry.people = model[key];
        result.push(entry);
    });
    return result;
}