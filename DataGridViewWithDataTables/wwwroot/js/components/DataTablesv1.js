
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


