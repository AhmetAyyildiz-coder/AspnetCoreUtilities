
function InitialDataTableWithServerSide(htmlTableName,postUrl,columsArray) {
    $(htmlTableName).DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: postUrl,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                return JSON.stringify({
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search.value,
                    searchRegex: d.search.regex,
                    columns: d.columns.map(function (col) {
                        return {
                            data: col.data,
                            searchValue: col.search.value,
                            searchRegex: col.search.regex,
                            searchable: col.searchable
                        };
                    }),
                    sortColumn: d.columns[d.order[0].column].data,
                    sortDirection: d.order[0].dir
                });
            }
        },
        searching: true,
        columns: columsArray,
       
        language: {
            url: '/lib/datatables-aspnetcore/turkish_config.json'
        }
    });
}




function InitialDataTableWithServerSideAndMultipleFilter(htmlTableName, postUrl, columsArray) {
    $(htmlTableName).DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: postUrl,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                return JSON.stringify({
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search.value,
                    searchRegex: d.search.regex,
                    columns: d.columns.map(function (col) {
                        return {
                            data: col.data,
                            searchValue: col.search.value,
                            searchRegex: col.search.regex,
                            searchable: col.searchable
                        };
                    }),
                    sortColumn: d.columns[d.order[0].column].data,
                    sortDirection: d.order[0].dir
                });
            }
        },
        searching: true,
        columns: columsArray,

        language: {
            url: '/lib/datatables-aspnetcore/turkish_config.json'
        },

        // burası çok önemli. Çoklu açılır liste şeklinde filtre ekleyeceğiz.
        initComplete: function () {
            this.api().columns().every(function () {
                var column = this;
                if (column[0][0] != 0) {
                    var select = $('<select><option value=""></option></select>')
                        .appendTo($(column.header()))
                        .on('change', function () {
                            var val = $.fn.dataTable.util.escapeRegex(
                                $(this).val()
                            );
                            column
                                .search(val ? '^' + val + '$' : '', true, false)
                                .draw();
                        });
                    column.data().unique().sort().each(function (d, j) {
                        select.append('<option value="' + d + '">' + d + '</option>')
                    });
                }
            });
        }
            


    });
}





