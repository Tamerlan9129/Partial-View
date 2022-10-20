$(function () {
    var skipRow = 1;
    $(document).on('click', '#loadMore', function () {
        $.ajax({
            method: "GET",
                url: "/home/loadMore",
                data: {
                skipRow : skipRow
            },
            success: function (result) {
                $('#recentWorkComponents').append(result);
                skipRow++;
            }


        })
    })


})

$(function () {
    var skipRow = 0;
    $(document).on('click', '#loader', function () {
        $.ajax({
            method: "GET",
            url: "/pricing/loadMore",
            data: {
                skipRow: skipRow
            },
            success: function (result) {
                $('#pricinComponents').append(result);
                skipRow++;
            }
        })
    })
})