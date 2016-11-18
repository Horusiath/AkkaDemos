/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="typings/google-maps/google-maps.d.ts" />
var demo;
(function (demo) {
    function position(p) {
        return new google.maps.LatLng(p.latitude, p.longitude);
    }
    var Vehicle = (function () {
        function Vehicle(vehiclePosition, maps, onClick) {
            var _this = this;
            this.id = vehiclePosition.identifier;
            var icon = {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 4,
                fillColor: "#FF0000",
                fillOpacity: 1,
                strokeWeight: 1,
                rotation: 0
            };
            var marker = new google.maps.Marker({ map: maps, icon: icon });
            marker.addListener('click', function () { return onClick(_this); });
            this.marker = marker;
            this.marker.setPosition(position(vehiclePosition.position));
        }
        Vehicle.prototype.setPosition = function (p) {
            this.marker.setPosition(position(p));
        };
        return Vehicle;
    }());
    var App = (function () {
        function App() {
            var _this = this;
            this.vehicleHistory = function (vehicle) {
                _this.server.getHistory(vehicle.id)
                    .then(function (response) {
                    if (response.positions) {
                        for (var i = 0; i < response.positions.length; i++) {
                            response.positions[i].when = new Date(response.positions[i].when);
                        }
                        _this.currentHistory(response.positions);
                    }
                });
            };
            this.updateVehicle = function (p) {
                var vehicle = _this.lookup[p.identifier];
                if (!vehicle) {
                    vehicle = new Vehicle(p, _this.map, _this.vehicleHistory);
                    _this.lookup[p.identifier] = vehicle;
                    _this.vehicles.push(vehicle);
                }
                vehicle.setPosition(p.position);
            };
            this.init = function () {
                _this.currentHistory = ko.observable();
                var hub = $.connection['vehicleHub'];
                _this.server = hub.server;
                hub.client.positionChanged = function (vp) {
                    _this.updateVehicle(vp);
                };
                $.connection.hub.start().then(function () {
                    _this.map = new google.maps.Map(document.getElementById('map'), {
                        zoom: 13,
                        center: new google.maps.LatLng(57.70887000, 11.97456000)
                    });
                    _this.vehicles = ko.observableArray([]);
                    _this.lookup = {};
                    ko.applyBindings(_this, document.body);
                });
            };
        }
        return App;
    }());
    demo.App = App;
})(demo || (demo = {}));
var app = new demo.App();
for (var key in Object.keys(app)) {
    if (Object.hasOwnProperty(key) && typeof app[key] == "function") {
        app[key] = app[key].bind(app);
    }
}
$(app.init);
