using FlickrNet;
using SideStream.API.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SideStream.API.Services
{
    public class FlickrService : IOC.IPointLayerProvider
    {
        public MashupDataSource DataSource
        {
            get { return MashupDataSource.Flickr; }
        }

        Flickr _api;
        private Flickr api
        {
            get
            {
                if(_api == null)
                {
                    _api = new Flickr();
                    _api.InstanceCacheDisabled = true;
                }
                return _api;
            }
        }

        public PointLayer GetPointLayerByBounds(double neLat, double neLng, double swLat, double swLng, int page = 1, string[] ds = null, string[] tags = null)
        {

            PhotoSearchOptions searchOptions = new PhotoSearchOptions();

            searchOptions.Licenses.Add(LicenseType.AttributionCC);
            searchOptions.Licenses.Add(LicenseType.AttributionShareAlikeCC);
            searchOptions.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
            searchOptions.Licenses.Add(LicenseType.NoKnownCopyrightRestrictions);

            BoundaryBox bndry = new BoundaryBox();

            if (neLat > swLat)
            {
                bndry.MinimumLatitude = swLat;
                bndry.MaximumLatitude = neLat;
            }
            else
            {
                bndry.MinimumLatitude = neLat;
                bndry.MaximumLatitude = swLat;
            }

            if (neLng > swLng)
            {
                bndry.MinimumLongitude = swLng;
                bndry.MaximumLongitude = neLng;
            }
            else
            {
                bndry.MinimumLongitude = neLng;
                bndry.MaximumLongitude = swLng;
            }

            searchOptions.BoundaryBox = bndry;

            if (!(tags == null || tags.Length < 1))
                searchOptions.Tags =  string.Join(",",tags);

            searchOptions.Accuracy = GeoAccuracy.Street;

            searchOptions.PerPage = 50;
            searchOptions.SortOrder = PhotoSearchSortOrder.InterestingnessDescending;

            searchOptions.Extras |= PhotoSearchExtras.Geo;
            searchOptions.Extras |= PhotoSearchExtras.OwnerName;
            searchOptions.Extras |= PhotoSearchExtras.CountFaves;
            searchOptions.Extras |= PhotoSearchExtras.DateTaken;
            searchOptions.Extras |= PhotoSearchExtras.Description;
            searchOptions.Extras |= PhotoSearchExtras.Tags;
            searchOptions.Extras |= PhotoSearchExtras.SquareUrl;

            searchOptions.Page = page;

            PhotoCollection flickrPhotos = api.PhotosSearch(searchOptions);

            PointLayer result = new PointLayer();
            result.DataSource = this.DataSource;
            result.TotalResults = flickrPhotos.Total;
            result.Page = flickrPhotos.Page;
            result.PageCount = flickrPhotos.Pages;
            result.Points = PointsFromPhotoCollection(flickrPhotos);

            return result;
        }

        private IList<Point> PointsFromPhotoCollection(PhotoCollection photos)
        {
            IList<Point> points = new List<Point>();

            foreach (Photo p in photos)
            {
                Point point = new Point(this.DataSource);
                point.Id = p.PhotoId;
                point.Title = p.Title;
                point.Latitude = p.Latitude;
                point.Longitude = p.Longitude;

                point.Properties.Add("desc",p.Description);
                point.Properties.Add("owner",p.OwnerName);
                point.Properties.Add("taken",p.DateTaken.ToString("MMM d, yyyy @h:mtt"));
                point.Properties.Add("likes",p.CountFaves);
                point.Properties.Add("thumbnail",p.LargeSquareThumbnailUrl);
                //point.Properties.Add("thumbnail", p.SquareThumbnailUrl);

                 if (!string.IsNullOrEmpty(p.LargeUrl))
                       point.Properties.Add("image",p.LargeUrl);
                 else if (!string.IsNullOrEmpty(p.MediumUrl))
                        point.Properties.Add("image",p.MediumUrl);
                    else
                        point.Properties.Add("image",p.SmallUrl);

                point.AddTags(p.Tags);

                points.Add(point);
            }

            return points;
        }

    }
}