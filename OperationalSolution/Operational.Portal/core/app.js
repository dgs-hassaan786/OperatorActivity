'use strict';
function initializeApp() {
    angular.bootstrap(document, ['dwdOperational']);
}

var ngApp = (function (initializeApp) {
    var app = angular.module('dwdOperational', [])
     .config(['$controllerProvider', '$compileProvider', '$filterProvider', '$provide', function ($controllerProvider, $compileProvider, $filterProvider, $provide) {
         app.controller = $controllerProvider.register;
         app.directive = $compileProvider.directive;
         app.filter = $filterProvider.register;
         app.factory = $provide.factory;
         app.service = $provide.service;
     }])
    .run(['$window', '$rootScope', function ($window, $rootScope) {

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

        $scope.getRecords = function () {

            MethodProvider.get('http://localhost:62287/api/operational').then(function (result) {
                var data = result.data;
                $scope.operationalReportData = data;

            }, function (err) {

                console.error('got error in getrecords: ', err);
            })
        }

    });

    if (typeof initializeApp != undefined && typeof initializeApp === 'function') {
        initializeApp();
    }

})(initializeApp);