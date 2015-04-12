// var NIC4Outdoors = NIC4Outdoors || {};


NIC4Outdoors.Maps = {

}

// vvvvvvvvvvvvvvvvvvvv  LayerManager  vvvvvvvvvvvvvvvvvvvv
NIC4Outdoors.Maps.LayerManager = function(googleMap){
    
    // ----- Members -----
    var map = googleMap;
    var that = this;
    var layers = {}; // Hashmap of NIC4Outdoors.Maps.Layer
    var layer;
    var infoWindow = new google.maps.InfoWindow({
        pixelOffset: new google.maps.Size(0, 0),
        content: ""
    });
    

    var getContent;
    var infoBox = new InfoBox({
        content: '',
        disableAutoPan: false,
        maxWidth: 265,
        pixelOffset: new google.maps.Size(-76, -195),
        zIndex: null,
        boxStyle: {
            background: "none"
		  , opacity: 1
		  , width: "265px"
        },
        closeBoxMargin: "12px 4px 2px 2px",
        closeBoxURL: "",
        infoBoxClearance: new google.maps.Size(1, 1)
    });
    var closeMarkers = [];
    var closeMarkersIndex = -1;
    var closeMarkersPixelRadius = 40;


    var tags = [],
        selectedTags = [];
    tags.selectedTags = [];

    var tagsList = $('#tags'),
        filtersList = $('#filters'),
        selectedFilters = [],
        currentPage = 1;

    function checkFilters(){
        selectedFilters = [];
        filtersList.find('[data-layer].active').each(function(){
            selectedFilters.push($(this).attr('data-layer'));
        });
    }

    // ----- Public Methods -----
    this.setInfoWindowContentMethod = function (func) {
        getContent = func;
    };

    this.refreshLayers = function (clear)
    {
        if (!clear)
            clear = false;

        checkFilters();

        var bounds = map.getBounds();
        var neLatLng = bounds.getNorthEast();
        var swLatLng = bounds.getSouthWest();

        var apiMethod = NIC4Outdoors.apiPath + 'maps/layers/get';
        var getData = {
            neLat: neLatLng.lat(),
            neLng: neLatLng.lng(),
            swLat: swLatLng.lat(),
            swLng: swLatLng.lng(),
            ds: selectedFilters,
            page: currentPage,
            tags: tags.selectedTags
        }
        console.log(selectedFilters);

        //var promise = $.getJSON(apiMethod,getData);
        //promise.then(function (data) { loadLayers(data, clear); });

        $.ajax({
            url: apiMethod,
            data: getData,
            traditional: true,
            success: function (data) { loadLayers(data, clear); }
        });


        removeUnshownMarkers();
    };

    this.closeInfoWindow = function()
    {
        infoBox.close();
    };


    this.getMarkerCount = function()
    {
        return closeMarkers.length;
    };
    this.getMarkerIndex = function()
    {
        return closeMarkersIndex;
    };
    this.previousMarker = function()
    {
        if(closeMarkers.length <= 1)
            return;

        var index = closeMarkersIndex;
        if(index == 0)
            index = closeMarkers.length -1;
        else
            index --;
            
        infoBox.close();
        markerClick({
            feature: closeMarkers[index].feature,
            latLng: closeMarkers[index].feature.getGeometry().get()
        }, false);

        closeMarkersIndex = index;
    };
    this.nextMarker = function()
    {
        if(closeMarkers.length <= 1)
            return;

        var index = closeMarkersIndex + 1;
        if(!(index < closeMarkers.length))
            index = 0;
            
        infoBox.close();

        markerClick({
            feature: closeMarkers[index].feature,
            latLng: closeMarkers[index].feature.getGeometry().get()
        }, false);

        closeMarkersIndex = index;
    };

    // ----- Private Methods -----
    function loadLayers(data,clear)
    {
        // JSON Data
        // layers: [{type,ds,cnt,pg,pgCnt,data}]
        // tags: [{tag,cnt}] <<- Maybe remove  

        $.each(data.layers,function(index,value){
            loadLayer(value,clear);
        });

    }

    function loadLayer(jsonLayer,clear)
    {

        // Existing Layer
        layer = layers[jsonLayer.ds];

        // New Layer
        if(!layer)
        {
            layer =  new NIC4Outdoors.Maps.Layer(jsonLayer.ds,jsonLayer.type);
            layer.mapLayer.addListener('click', function (event) { markerClick(event); });
            layer.mapLayer.setMap(map);
            layers[jsonLayer.ds] = layer;
        }

        // $.each(jsonLayer.data.features, function(index,value) {
        //     // console.log(layer.mapLayer);
        //     if(layer.mapLayer.getFeatureById(value.id)) {
                
        //     }
        // })

        var filteredData = $.grep(jsonLayer.data.features, function(value,index){
            // console.log(layer.mapLayer.getFeatureById(value.id) == true);
            if(layer.mapLayer.getFeatureById(value.id)) {
                // console.log('true');
                return false;
            }
            else {
                 // console.log('false');
                return true;
            }
        });
        // console.log(filteredData)
        jsonLayer.data.features = filteredData;

        layer.count = jsonLayer.cnt;
        layer.page = jsonLayer.pg;
        layer.pages = jsonLayer.pgCnt;

        if(clear)
            layer.clear();

        layer.mapLayer.addGeoJson(jsonLayer.data);



        tagsList.html('');
        // console.log(jsonLayer.data.features);

        // Build tags variable and get count
        tags = [];
        $.each(jsonLayer.data.features,function(index,value){

            var dataTags = value.properties.tags;

            // Build count for each tag
            $.each(dataTags,function(index,value){
                if(!tags[value] && tags[value] != 0) tags[value] = 1;
                else tags[value] += 1;
            })

        })

        // Display the tags
        for (var name in tags) {
          if (tags.hasOwnProperty(name)) {
            var count = tags[name];
            var active = '';
            // console.log($.inArray(name,selectedTags), selectedTags, name);
            if($.inArray(name,selectedTags) >= 0) active = 'class="active"';
            // console.log(selectedTags);
            if(count > 0) tagsList.append('<li data-tag="'+name+'"'+active+'>'+name+' <span class="count">'+count+'</span></li>');
          }
        }
        tagsList.find('.active').prependTo(tagsList);

        // console.log(tags);


    }


    

    function removeUnshownMarkers()
    {
        mapBounds = map.getBounds();
        $.each(layers, function (ds, layer) {
            if (layer.type == 'PointLayer') {
                layer.mapLayer.forEach(function (feature) {
                    if (!mapBounds.contains(feature.getGeometry().get())) {
                        layer.mapLayer.remove(feature);
                    }
                });
            }
        });
    }

    function markerClick(event, userClick) {

        userClick = typeof userClick !== 'undefined' ? userClick : true;

        var feature = event.feature;

        // !!! WARNING !!! feature.k is undocumented and could change but there is no api equivelant
        // infoWindow.setContent($.render.FlickrMapInfo(feature.k));

        // ToDo: Add templating to info window
        //infoWindow.setContent(getContent(feature.k));
        infoBox.setContent(getContent(feature.k));

        //var anchor = new google.maps.MVCObject();
       
        //anchor.set('position', event.latLng);

        var anchor = new google.maps.Marker({ position: event.latLng });
        //anchor.setPosition(event.latLng);
        infoBox.open(map, anchor);

        if(userClick)
            getCloseMarkers(feature);
    }

    function getCloseMarkers(feature)
    {
        closeMarkers = [];
        closeMarkers.push({distance:0,feature:feature});

        $.each(layers, function (ds, layer) {
            if (layer.type == 'PointLayer') {
                layer.mapLayer.forEach(function (feature2) {
                    pDist = feature.pixelDistance(feature2,map.getZoom());
                    if(Math.abs(pDist) <= closeMarkersPixelRadius)
                    {
                        closeMarkers.push({distance:pDist,feature:feature2});
                    }
                });
            }
        });

        closeMarkers.sort(function(a,b){
            return ((a.distance < b.distance) ? -1 : ((a.distance > b.distance) ? 1 : 0));
        });

        closeMarkersIndex = -1;
        for(i=0;closeMarkersIndex < 0;i++)
        {
            if(closeMarkers[i].distance == 0)
            {
                closeMarkersIndex = i;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    // Filters
    ////////////////////////////////////////////////////////////////////////////////    
    filtersList.find('[data-layer]').click(function(){

        var id = $(this).data('layer');
        $(this).toggleClass('active');

        if($(this).hasClass('active')) {
            layer.mapLayer.setMap(map);
        }
        else {
            layer.mapLayer.setMap(null);
            layerManager.closeInfoWindow();
        }

        checkFilters();

        that.refreshLayers();

        return false;

    });


    ////////////////////////////////////////////////////////////////////////////////
    // Load More / Paging
    ////////////////////////////////////////////////////////////////////////////////  
    $('#load-more').click(function(){
        currentPage += 1;
        that.refreshLayers();
        return false;
    });



    ////////////////////////////////////////////////////////////////////////////////
    // Tag Filtering
    ////////////////////////////////////////////////////////////////////////////////  
    tagsList.on('click','li',function(){

        var tagName = $(this).attr('data-tag');
        $(this).toggleClass('active');

        tags.selectedTags = [];
        selectedTags = [];
        tagsList.find('.active').each(function(){
            tags.selectedTags.push($(this).attr('data-tag'));
            selectedTags.push($(this).attr('data-tag'));
        });

        that.refreshLayers();

        $.each(layers, function (ds, layer) {
            if (layer.type == 'PointLayer') {
                layer.mapLayer.forEach(function (feature) {
                    // console.log(feature);
                    if ($.inArray(tagName,feature.k.tags) < 0) {
                        layer.mapLayer.remove(feature);
                    }
                });
            }
        });
        
    })


  
};
// ^^^^^^^^^^^^^^^^^^^^  LayerManager  ^^^^^^^^^^^^^^^^^^^^

// vvvvvvvvvvvvvvvvvvvv  Layer  vvvvvvvvvvvvvvvvvvvv
NIC4Outdoors.Maps.Layer = function(ds,type){
    this.ds = ds;
    this.type = type;
    this.mapLayer = new google.maps.Data();
    this.count = 0;
    this.page = 0;
    this.pages = 0;
    var that = this;
    this.clear = function(){
        this.mapLayer.forEach(function (feature) {
            that.mapLayer.remove(feature);
        });
    };
};
// ^^^^^^^^^^^^^^^^^^^^  Layer  ^^^^^^^^^^^^^^^^^^^^


var pdOFFSET = 268435456;
var pdRADIUS = 85445659.4471;

function lonToX(lon) {
    return Math.round(pdOFFSET + pdRADIUS * lon * Math.PI / 180);
}

function latToY(lat) {
    return Math.round(pdOFFSET - pdRADIUS *
                Math.log((1 + Math.sin(lat * Math.PI / 180)) /
                (1 - Math.sin(lat * Math.PI / 180))) / 2);
}

$(function () {

    google.maps.Data.Feature.prototype.pixelDistance = function (toFeature, zoomLevel)
    {
        var x1 = lonToX(this.getGeometry().get().lng());
        var y1 = latToY(this.getGeometry().get().lat());

        var x2 = lonToX(toFeature.getGeometry().get().lng());
        var y2 = latToY(toFeature.getGeometry().get().lat());

        var dist = Math.sqrt(Math.pow((x1 - x2), 2) + Math.pow((y1 - y2), 2)) >> (21 - zoomLevel);
        if(x1>x2)
            dist = dist * -1;

        return dist;
    };

});





