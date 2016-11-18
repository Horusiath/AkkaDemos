/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="typings/google-maps/google-maps.d.ts" />

module demo {

    interface IPosition {
        latitude: number;
        longitude: number;
    }

    interface ISignalPosition {
        identifier: string;
        position: IPosition;
    }

    interface IPositionChanged {
        when: string;
        position: IPosition;
    }

    interface IPositionServer {
        getHistory(vehicleId: string): JQueryPromise
    }

    function position(p: IPosition): google.maps.LatLng {
        return new google.maps.LatLng(p.latitude, p.longitude);
    }

    class Vehicle {
        public id: string;
        private marker: google.maps.Marker;

        constructor(vehiclePosition: ISignalPosition, maps: google.maps.Map, onClick: Function) {
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
            (<any>marker).addListener('click', () => onClick(this));
            this.marker = marker;
            this.marker.setPosition(position(vehiclePosition.position));
        }

        setPosition(p: IPosition) {
            this.marker.setPosition(position(p));
        }
    }

    export class App {
        private server: IPositionServer;
        public vehicles: KnockoutObservableArray<Vehicle>;
        private map: google.maps.Map;
        private lookup: any;
        public currentHistory: KnockoutObservable<IPositionChanged[]>;

        public vehicleHistory = (vehicle: Vehicle) => {
            this.server.getHistory(vehicle.id)
                .then(response => {
                    if (response.positions) {
                        for (var i = 0; i < response.positions.length; i++) {
                            response.positions[i].when = new Date(response.positions[i].when);
                        }
                        this.currentHistory(response.positions);
                    }
                });
        };
       
        public updateVehicle = (p: ISignalPosition) => {
            var vehicle: Vehicle = this.lookup[p.identifier];
            if (!vehicle) {
                vehicle = new Vehicle(p, this.map, this.vehicleHistory);
                this.lookup[p.identifier] = vehicle;
                this.vehicles.push(vehicle);
            }

            vehicle.setPosition(p.position);
        };

        public init = () => {
            this.currentHistory = ko.observable<IPositionChanged[]>();
            var hub = $.connection['vehicleHub'];
            this.server = hub.server;
            hub.client.positionChanged = (vp) => {
                this.updateVehicle(vp);
            };
            $.connection.hub.start().then(() => {
                this.map = new google.maps.Map(document.getElementById('map'), {
                    zoom: 13,
                    center: new google.maps.LatLng(57.70887000, 11.97456000)
                });
                this.vehicles = ko.observableArray([]);
                this.lookup = {};
                ko.applyBindings(this, document.body);
            });
        };
    }
}

var app = new demo.App();
for (var key in Object.keys(app)) {
    if (Object.hasOwnProperty(key) && typeof app[key] == "function") {
        app[key] = app[key].bind(app);
    }
}

$(app.init);