
function InitialDataTableWithServerSide(htmlTableName, postUrl, columsArray) {
    alert('test')
    $(htmlTableName).DataTable({
        processing: true,
        serverSide: true,
        sorting: false, // Sıralamayı devre dışı bırakır
        ajax: {
            url: postUrl,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                console.log(d);
                return JSON.stringify({
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search.value,
                    searchRegex: d.search.regex,
                    columns: d.columns.map(function (col) {
                        debugger;
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
        ordering: false, // Sıralamayı devre dışı bırakır
        paging: false,          // Sayfalama kapatılır
        scrollY: '300px',       // Dikey scroll eklenir (300px yüksekliğinde)
        scrollX: true,          // Yatay scroll eklenir
        scrollCollapse: true,   // Scroll alanı içeriğe göre daraltılabilir
        ajax: {
            url: postUrl,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                // Her sütun için ayrı filtre değerlerini al
                var columnFilters = d.columns.map(function (column) {
                    return {
                        data: column.data,
                        searchValue: column.search.value || '', // Boş string varsa default olarak boş bırak
                        searchable: column.searchable
                    };
                });

                return JSON.stringify({
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    globalSearchValue: d.search.value || '', // Global arama için
                    columns: columnFilters,
                    //sortColumn: d.columns[d.order[0].column].data,
                    //sortDirection: d.order[0].dir
                });
            }
        },

        searching: true,
        columns: columsArray,
        language: {
            url: '/lib/datatables-aspnetcore/turkish_config.json'
        },
        initComplete: function () {
            this.api().columns().every(function (index) {
                var column = this;
                // İlk sütun hariç diğer sütunlara filtre ekle
                if (index !== 0) {
                    var select = $('<select multiple="multiple" class="form-control select2"></select>')
                        .appendTo($(column.header()))
                        .select2({
                            placeholder: "Seçiniz",
                            allowClear: true
                        })
                        .on('change', function () {
                            var values = $(this).val();
                            var searchValue = values ? values.join('|') : '';
                            column.search(searchValue, true, false).draw();
                        });

                    column.data().unique().sort().each(function (d) {
                        // Null veya boş değerleri atla
                        if (d !== null && d !== '') {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        }
                    });
                }
            });
        }
    });
}





