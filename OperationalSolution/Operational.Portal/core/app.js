'use strict';
function initializeApp() {
    angular.bootstrap(document, ['dwdOperational']);
}

var ngApp = (function (initializeApp) {
    var app = angular.module('dwdOperational', ['smart-table', 'angularUtils.directives.dirPagination'])
     .config(['$controllerProvider', '$compileProvider', '$filterProvider', '$provide', function ($controllerProvider, $compileProvider, $filterProvider, $provide) {
         app.controller = $controllerProvider.register;
         app.directive = $compileProvider.directive;
         app.filter = $filterProvider.register;
         app.factory = $provide.factory;
         app.service = $provide.service;
     }])
    .run(['$window', '$rootScope', function ($window, $rootScope) {

    }]);

    //Main filters operations
    app.service('FilterService', function () {
        this.StartWith = function (items, char, key) {

            var filtered = [];
            if (char != "" && key != "" && typeof items != "undefined") {

                if (key == 'Created' || key == 'Modified') {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var date = $.datepicker.formatDate("dd M yy", new Date(item[key]));
                        if (date == char) {
                            filtered.push(item);
                        }

                    }

                } else {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var val = "" + item[key];
                        if (char.toString().substring(0, 1).toLowerCase() == val.substring(0, 1).toLowerCase()) {
                            filtered.push(item);
                        }
                    }
                }

                return filtered;
            } else {
                return items;
            }

        }
        this.EndWith = function (items, char, key) {
            var filtered = [];
            if (char != "" && key != "") {
                var letterMatch = new RegExp(char.substring(char.length - 1, char.length), 'i');
                if (key == 'Created' || key == 'Modified') {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var date = $.datepicker.formatDate("dd M yy", new Date(item[key]));
                        if (date == char) {
                            filtered.push(item);
                        }

                    }

                } else {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var val = "" + item[key];
                        if (char.toString().substring(char.length - 1, char.length).toLowerCase() == val.substring(item[key].length - 1, item[key].length).toLowerCase()) {
                            filtered.push(item);
                        }
                    }
                }
                return filtered;
            } else {
                return items;
            }
        }
        this.Contains = function (items, char, key) {
            var filtered = [];
            if (char != "" && key != "") {
                if (key == 'Created' || key == 'Modified') {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var date = $.datepicker.formatDate("dd M yy", new Date(item[key]));
                        if (date == char) {
                            filtered.push(item);
                        }

                    }

                } else {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var val = "" + item[key];
                        if (val.toString().toLowerCase().indexOf(char.toLowerCase()) !== -1) {
                            filtered.push(item);
                        }

                    }
                }
                return filtered;
            } else {
                return items;
            }
        }
        this.Equals = function (items, char, key) {
            var filtered = [];


            if (char != "" && key != "") {
                if (key == 'Created' || key == 'Modified') {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var date = $.datepicker.formatDate("dd M yy", new Date(item[key]));
                        if (date.toLowerCase() == char.toLowerCase()) {
                            filtered.push(item);
                        }

                    }

                } else {
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        if (item[key]) {
                            if (item[key].toString().toLowerCase() == char.toLowerCase()) {
                                filtered.push(item);
                            }
                        }

                    }
                }

                return filtered;
            } else {
                return items;
            }
        }
    });

    app.service('ExportService', function () {
        var self = this;
        self.export = function (name, columns, data) {
            var csvData = new Array();
            var keys = new Array();
            var first = data[0]
            for (var key in first) {
                if (columnIndex(key) != -1)
                    keys.push(key);
            }
            //csvData.push(keys.toString());
            //data.forEach(function (item, index, array) {
            //    csvData.push(toCSV(item, ','));
            //});

            var csvData = convertArrayOfObjectsToCSV({
                data: data
            });

            // download stuff
            var fileName = name + ".csv";
            var buffer = csvData; //.join("\n");
            var blob = new Blob([buffer], {
                "type": "text/csv;charset=utf8;"
            });
            var link = document.getElementById(name);
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                link.setAttribute("href", window.URL.createObjectURL(blob));
                link.setAttribute("download", fileName);
                $(link)[0].click();
            }

            function convertArrayOfObjectsToCSV(args) {
                var result, ctr, keys, columnDelimiter, lineDelimiter, data;

                data = args.data || null;
                if (data == null || !data.length) {
                    return null;
                }

                columnDelimiter = args.columnDelimiter || ',';
                lineDelimiter = args.lineDelimiter || '\n';

                keys = Object.keys(data[0]);

                result = '';
                result += keys.join(columnDelimiter);
                result += lineDelimiter;

                data.forEach(function (item) {
                    ctr = 0;
                    keys.forEach(function (key) {
                        if (ctr > 0)
                            result += columnDelimiter;

                        var str = "\"";
                        if (isNaN(item[key]) && item[key] != undefined && item[key] != null)
                            str += item[key].replace(/"/g, "\"\"");
                        else
                            str += item[key];

                        str += "\"";
                        result += str;

                        ctr++;
                    });
                    result += lineDelimiter;
                });

                return result;
            }

            function toCSV(obj, separator) {
                var arr = [];
                for (var key in obj) {
                    if (obj.hasOwnProperty(key)) {
                        if (columnIndex(key) != -1)
                            arr.push('\"' + obj[key] + '\"');
                    }
                }
                return arr.join(separator || ",");
            }

            function columnIndex(key) {
                var index = columns.findIndex(function (x) {
                    return x.field.toLowerCase() == key.toLowerCase() && x.show == true
                });
                return index;
            }
        }
        return self;
    });

    // Main Filter for Grid return list based on criteria , attached on tr element on grid
    app.filter('myFilter', ['FilterService', function (FilterService) {

        return function (items, options) {

            switch (options.selectedFilter) {
                case 'CT':
                    {
                        return FilterService.Contains(items, options.searchValue, options.selectedField);
                    }
                case 'EQ':
                    {
                        return FilterService.Equals(items, options.searchValue, options.selectedField);
                    }
                case 'ST':
                    {
                        return FilterService.StartWith(items, options.searchValue, options.selectedField);
                    }
                case 'ET':
                    {
                        return FilterService.EndWith(items, options.searchValue, options.selectedField);
                    }

                case 'Contains':
                    {
                        return FilterService.Contains(items, options.searchValue, options.selectedField);
                    }
                case 'Equals':
                    {
                        return FilterService.Equals(items, options.searchValue, options.selectedField);
                    }
                case 'StartWith':
                    {
                        return FilterService.StartWith(items, options.searchValue, options.selectedField);
                    }
                case 'EndWith':
                    {
                        return FilterService.EndWith(items, options.searchValue, options.selectedField);
                    }

                default:
                    {
                        return items;
                    }

            }
            //alert(options.searchValue + '-' + options.selectedField + '-' + options.selectedFilter);
            //return items;
        }
    }]);


    app.service('MethodProvider', ['$http', function ($http) {
        var self = this;

        self.get = function (url) {
            var obj = {
                url: url,
                method: 'GET',
                dataType: 'jsonp',
                async: true,
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            return $http(obj);
        };

        self.getPagingData = function (url, pageNo, pageSize) {
            var obj = {
                url: url,
                method: 'GET',
                async: true,
                params: { pageNo: pageNo, pageSize: pageSize },
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            return $http(obj);
        };

        self.post = function (url, data) {
            var obj = {
                url: url,
                method: 'POST',
                async: true,
                data: JSON.stringify(data),
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            return $http(obj);
        };

        self.put = function (url, data) {
            var obj = {
                url: url,
                method: 'PUT',
                async: true,
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            if (typeof data != 'undefined' && data != null) {
                obj.data = JSON.stringify(data);
            }
            return $http(obj);
        };

        self.delete = function (url) {
            var obj = {
                url: url,
                method: 'POST',
                async: true,
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            return $http(obj);
        };
    }]);
    
    app.controller('OperationalController', function ($scope, MethodProvider) {

        $scope.operationalReportData = [];
        $scope.distinctWebsite = [];
        $scope.distinctDevices = [];

        $scope.searchOption = {
            device: '',
            website: '',
            from: '',
            to: ''
        };

        $scope.getRecords = function () {

            MethodProvider.get('http://localhost:62287/api/operational').then(function (result) {
                var data = result.data;
                $scope.operationalReportData = data;

            }, function (err) {

                console.error('got error in getrecords: ', err);
            });
        }

        $scope.getDistinctWebsites = function () {

            MethodProvider.get('http://localhost:62287/api/operational/websites').then(function (result) {
                var data = result.data;
                $scope.distinctWebsite = data;
                if ($scope.distinctWebsite.length > 0) {
                    $scope.searchOption.website = $scope.distinctWebsite[0];
                }
            }, function (err) {

                console.error('got error in getrecords: ', err);
            });
        }

        $scope.getDistinctDevices = function () {

            MethodProvider.get('http://localhost:62287/api/operational/devices').then(function (result) {
                var data = result.data;
                $scope.distinctDevices = data;
                if ($scope.distinctDevices.length > 0) {
                    $scope.searchOption.device = $scope.distinctDevices[0];
                }

            }, function (err) {

                console.error('got error in getrecords: ', err);
            });
        }


        $scope.doAdvanceSearch = function () {
            debugger
            MethodProvider.post('http://localhost:62287/api/operational', {
                Device: $scope.searchOption.device,
                To: $scope.searchOption.to,
                From: $scope.searchOption.from,
                Website: $scope.searchOption.website
            }).then(function (result) {
                var data = result.data;
                $scope.operationalReportData = data;

            }, function (err) {

                console.error('got error in getrecords: ', err);
            });


        }

    });

    
    app.controller('NewOperatorController', function ($scope, $filter, ExportService) {
        //$scope.rowCollection = [];

        $scope.Filters = [{
            value: 'CT',
            filter: 'Contains'
        },
            {
                value: 'EQ',
                filter: 'Equals'
            },
            {
                value: 'ET',
                filter: 'Ends With'
            },
            {
                value: 'ST',
                filter: 'Starts With'
            }
        ];
        $scope.options = {
            selectedField: 'Name',
            selectedFilter: $scope.Filters[0].value,
            searchValue: '',
            page: 1,
            pagesize: 10,
            pagingOptions: [5, 10, 15, 20, 50, 100, 500, 1000],
            total: '',
            website: $scope.distinctWebsite[0],
            device: $scope.distinctDevices[0]
        };
        $scope.displayedCollection = [].concat($scope.$parent.operationalReportData);

        $scope.columns = [{
            'header': 'Operator Name',
            'field': 'Name',
            'show': true
        }, {
            'header': 'Proactive Sent',
            'field': 'ProactiveSent',
            'show': true
        }, { 
            'header': 'Proactive Answered',
            'field': 'ProactiveAnswered',
            'show': true
        }, {
            'header': 'Proactive Response Rate (%)',
            'field': 'ProactiveResponseRate',
            'show': true
        }, {
            'header': 'Reactive Received',
            'field': 'ReactiveReceived',
            'show': true
        }, {
            'header': 'Reactive Answered',
            'field': 'ReactiveAnswered',
            'show': true
        }, {
            'header': 'Reactive Response Rate (%)',
            'field': 'ReactiveResponseRate',
            'show': true
        }, {
            'header': 'Total Chat Length',
            'field': 'TotalChatLength',
            'show': true
        }, {
            'header': 'Average Chat Length',
            'field': 'AverageChatLength',
            'show': true
        }];       

        $scope.exportGrid = function () {
            var row = [];
            var col = [];
            col = angular.copy($scope.columns);
            row = angular.copy($scope.operationalReportData);
            ExportService.export("Build", col, row);
        }

        $scope.$watch('options.searchValue', function (newValue, oldValue) {
            if (!angular.equals(newValue, oldValue)) {
                var items = $filter('myFilter')($scope.operationalReportData, $scope.options);
                $scope.displayedCollection = angular.copy(items);
            }
        }, true);

        (function init() {
            var start = moment().subtract(29, 'days');
            var end = moment();

            function cb(start, end) {
                
                $scope.$parent.searchOption.from = start.format('YYYYMMDD');
                $scope.$parent.searchOption.to = end.format('YYYYMMDD');

                console.log("A new date range was chosen: " + start.format('YYYYMMDD') + ' to ' + end.format('YYYYMMDD'));
                $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
            }
            $('#reportrange').daterangepicker({
                startDate: start,
                endDate: end,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, cb);
            cb(start, end);
        })();
    });


    if (typeof initializeApp != undefined && typeof initializeApp === 'function') {
        initializeApp();
    }

})(initializeApp);