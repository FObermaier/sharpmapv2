using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public abstract class ProviderBase : IProvider
    {
        private static readonly PropertyDescriptorCollection _providerBaseTypeProperties;

        static ProviderBase()
        {
            _providerBaseTypeProperties = TypeDescriptor.GetProperties(typeof(ProviderBase));
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="ConnectionId"/> property.
        /// </summary>
        public static PropertyDescriptor ConnectionIdProperty
        {
            get { return _providerBaseTypeProperties.Find("ConnectionId", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="CoordinateTransformation"/> property.
        /// </summary>
        public static PropertyDescriptor CoordinateTransformationProperty
        {
            get { return _providerBaseTypeProperties.Find("CoordinateTransformation", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="IsOpen"/> property.
        /// </summary>
        public static PropertyDescriptor IsOpenProperty
        {
            get { return _providerBaseTypeProperties.Find("IsOpen", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="SpatialReference"/> property.
        /// </summary>
        public static PropertyDescriptor SpatialReferenceProperty
        {
            get { return _providerBaseTypeProperties.Find("SpatialReference", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="Srid"/> property.
        /// </summary>
        public static PropertyDescriptor SridProperty
        {
            get { return _providerBaseTypeProperties.Find("Srid", false); }
        }

        private readonly ICustomTypeDescriptor _descriptor;
        private Boolean _isDisposed;
        private Boolean _isOpen;
        private ICoordinateTransformation _coordinateTransform;
        private PropertyDescriptorCollection _instanceProperties;
        private Dictionary<PropertyDescriptor, Object> _propertyValues;

        protected ProviderBase() : this(null) { }

        protected ProviderBase(ICustomTypeDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        ~ProviderBase()
        {
            Dispose(false);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating if the object has been disposed.
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        protected abstract void Dispose(Boolean disposing);

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation of ICustomTypeDescriptor

        public virtual AttributeCollection GetAttributes()
        {
            return _descriptor != null ? _descriptor.GetAttributes() : null;
        }

        public virtual String GetClassName()
        {
            return _descriptor != null ? _descriptor.GetClassName() : null;
        }

        public virtual String GetComponentName()
        {
            return _descriptor != null ? _descriptor.GetComponentName() : null;
        }

        public virtual TypeConverter GetConverter()
        {
            return _descriptor != null ? _descriptor.GetConverter() : null;
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            return _descriptor != null ? _descriptor.GetDefaultEvent() : null;
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            return _descriptor != null ? _descriptor.GetDefaultProperty() : null;
        }

        public virtual Object GetEditor(Type editorBaseType)
        {
            return _descriptor != null ? _descriptor.GetEditor(editorBaseType) : null;
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            return _descriptor != null ? _descriptor.GetEvents() : null;
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return _descriptor != null ? _descriptor.GetEvents(attributes) : null;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return _instanceProperties ?? GetClassProperties();
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return _descriptor != null ? _descriptor.GetProperties(attributes) : null;
        }

        public virtual Object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _descriptor != null 
                ? _descriptor.GetPropertyOwner(pd) 
                : HasProperty(pd) 
                    ? this 
                    : null;
        }

        #endregion

        #region Implementation of IHasDynamicProperties

        public Int32 AddProperty(PropertyDescriptor property)
        {
            ensureInstanceProperties();
            return _instanceProperties.Add(property);
        }

        public Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            ensureInstanceProperties();
            Int32 index = _instanceProperties.Add(property);
            SetPropertyValueInternal(property, value);
            return index;
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal<TValue>(property);
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal(property);
        }

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public virtual Boolean HasProperty(PropertyDescriptor property)
        {
            PropertyDescriptorCollection properties = _instanceProperties ?? GetClassProperties();
            return properties.Contains(property) || (_propertyValues != null &&
                                                     _propertyValues.ContainsKey(property));
        }

        #endregion

        #region IProvider Members

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransform; } 
            set
            {
                if (_coordinateTransform != value)
                {
                    _coordinateTransform = value;
                    OnPropertyChanged(CoordinateTransformationProperty);
                }
            }
        }

        public abstract ICoordinateSystem SpatialReference { get; }

        public virtual Boolean IsOpen
        {
            get { return _isOpen; }
            private set
            {
                _isOpen = value;
                OnPropertyChanged(IsOpenProperty);
            }
        }

        public abstract Int32? Srid { get; }

        public abstract IExtents GetExtents();

        public abstract String ConnectionId { get; }

        public virtual void Open()
        {
            IsOpen = true;
        }

        public virtual void Close()
        {
            IsOpen = false;
        }

        public abstract Object ExecuteQuery(Expression query);

        #endregion

        protected abstract PropertyDescriptorCollection GetClassProperties();

        protected virtual TValue GetPropertyValueInternal<TValue>(PropertyDescriptor property)
        {
            checkGetValueType<TValue>(property);

            return (TValue)GetPropertyValueInternal(property);
        }

        protected virtual Object GetPropertyValueInternal(PropertyDescriptor property)
        {
            PropertyDescriptorCollection classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                GetObjectProperty(propertyName);
            }

            if (_instanceProperties.Contains(property))
            {
                Object value;

                return _propertyValues != null && _propertyValues.TryGetValue(property, out value)
                           ? value
                           : null;
            }

            throw new InvalidOperationException("Property doesn't exist in this instance: " +
                                                property.Name);
        }

        protected virtual void SetPropertyValueInternal<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);

            SetPropertyValueInternal(property, (Object)value);
        }

        protected virtual void SetPropertyValueInternal(PropertyDescriptor property, Object value)
        {
            PropertyDescriptorCollection classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                SetObjectProperty(propertyName, value);
            }
            else if (_instanceProperties.Contains(property))
            {
                if (_propertyValues == null)
                {
                    _propertyValues = new Dictionary<PropertyDescriptor, Object>();
                }

                _propertyValues[property] = value;
            }
            else
            {
                throw new InvalidOperationException("Property doesn't exist on this layer: " +
                                                    property.Name);
            }
        }

        protected virtual void SetObjectProperty(String propertyName, Object value)
        {
            if (propertyName.Equals(ConnectionIdProperty.Name) ||
                propertyName.Equals(IsOpenProperty.Name) ||
                propertyName.Equals(SridProperty.Name) ||
                propertyName.Equals(SpatialReferenceProperty.Name))
            {
                throw new InvalidOperationException("The property '" + propertyName + "' is read-only.");
            }

            if (propertyName.Equals(CoordinateTransformation.Name))
            {
                CoordinateTransformation = value as ICoordinateTransformation;
            }
        }

        protected virtual Object GetObjectProperty(String propertyName)
        {
            if (propertyName.Equals(CoordinateTransformation.Name))
            {
                return CoordinateTransformation;
            }

            if (propertyName.Equals(ConnectionIdProperty.Name))
            {
                return ConnectionId;
            }

            if (propertyName.Equals(IsOpenProperty.Name))
            {
                return IsOpen;
            }

            if (propertyName.Equals(SridProperty.Name))
            {
                return Srid;
            }

            if (propertyName.Equals(SpatialReferenceProperty.Name))
            {
                return SpatialReference;
            }

            return null;
        }

        protected void OnPropertyChanged(PropertyDescriptor property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property.Name));
            }
        }

        #region Private helper methods
        private static void checkGetValueType<TValue>(PropertyDescriptor property)
        {
            if (!typeof(TValue).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("The type of the property isn't " +
                                            "assignable to the value variable.");
            }
        }

        private static void checkSetValueType<TValue>(PropertyDescriptor property)
        {
            if (!property.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException("The type of the value isn't " +
                                            "assignable to the property.");
            }
        }

        private void ensureInstanceProperties()
        {
            if (_instanceProperties != null)
            {
                return;
            }

            PropertyDescriptorCollection classProperties = GetClassProperties();
            PropertyDescriptor[] propArray = new PropertyDescriptor[classProperties.Count];
            classProperties.CopyTo(propArray, 0);
            _instanceProperties = new PropertyDescriptorCollection(propArray, false);
        }

        private void checkPropertyParameter(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            if (!HasProperty(property))
            {
                throw new InvalidOperationException("Property doesn't exist for instance " +
                                                    ToString());
            }

            if (property.IsReadOnly)
            {
                throw new InvalidOperationException(String.Format("Property {0} is read only.",
                                                                  property.Name));
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public abstract class ProviderBase : IProvider
    {
        private static readonly PropertyDescriptorCollection _providerBaseTypeProperties;

        static ProviderBase()
        {
            _providerBaseTypeProperties = TypeDescriptor.GetProperties(typeof(ProviderBase));
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="ConnectionId"/> property.
        /// </summary>
        public static PropertyDescriptor ConnectionIdProperty
        {
            get { return _providerBaseTypeProperties.Find("ConnectionId", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="CoordinateTransformation"/> property.
        /// </summary>
        public static PropertyDescriptor CoordinateTransformationProperty
        {
            get { return _providerBaseTypeProperties.Find("CoordinateTransformation", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="IsOpen"/> property.
        /// </summary>
        public static PropertyDescriptor IsOpenProperty
        {
            get { return _providerBaseTypeProperties.Find("IsOpen", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="SpatialReference"/> property.
        /// </summary>
        public static PropertyDescriptor SpatialReferenceProperty
        {
            get { return _providerBaseTypeProperties.Find("SpatialReference", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="Srid"/> property.
        /// </summary>
        public static PropertyDescriptor SridProperty
        {
            get { return _providerBaseTypeProperties.Find("Srid", false); }
        }

        private readonly ICustomTypeDescriptor _descriptor;
        private Boolean _isDisposed;
        private Boolean _isOpen;
        private ICoordinateTransformation _coordinateTransform;
        private PropertyDescriptorCollection _instanceProperties;
        private Dictionary<PropertyDescriptor, Object> _propertyValues;

        protected ProviderBase() : this(null) { }

        protected ProviderBase(ICustomTypeDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        ~ProviderBase()
        {
            Dispose(false);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating if the object has been disposed.
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        protected abstract void Dispose(Boolean disposing);

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation of ICustomTypeDescriptor

        public virtual AttributeCollection GetAttributes()
        {
            return _descriptor != null ? _descriptor.GetAttributes() : null;
        }

        public virtual String GetClassName()
        {
            return _descriptor != null ? _descriptor.GetClassName() : null;
        }

        public virtual String GetComponentName()
        {
            return _descriptor != null ? _descriptor.GetComponentName() : null;
        }

        public virtual TypeConverter GetConverter()
        {
            return _descriptor != null ? _descriptor.GetConverter() : null;
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            return _descriptor != null ? _descriptor.GetDefaultEvent() : null;
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            return _descriptor != null ? _descriptor.GetDefaultProperty() : null;
        }

        public virtual Object GetEditor(Type editorBaseType)
        {
            return _descriptor != null ? _descriptor.GetEditor(editorBaseType) : null;
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            return _descriptor != null ? _descriptor.GetEvents() : null;
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return _descriptor != null ? _descriptor.GetEvents(attributes) : null;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return _instanceProperties ?? GetClassProperties();
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return _descriptor != null ? _descriptor.GetProperties(attributes) : null;
        }

        public virtual Object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _descriptor != null 
                ? _descriptor.GetPropertyOwner(pd) 
                : HasProperty(pd) 
                    ? this 
                    : null;
        }

        #endregion

        #region Implementation of IHasDynamicProperties

        public Int32 AddProperty(PropertyDescriptor property)
        {
            ensureInstanceProperties();
            return _instanceProperties.Add(property);
        }

        public Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            ensureInstanceProperties();
            Int32 index = _instanceProperties.Add(property);
            SetPropertyValueInternal(property, value);
            return index;
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal<TValue>(property);
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal(property);
        }

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public virtual Boolean HasProperty(PropertyDescriptor property)
        {
            PropertyDescriptorCollection properties = _instanceProperties ?? GetClassProperties();
            return properties.Contains(property) || (_propertyValues != null &&
                                                     _propertyValues.ContainsKey(property));
        }

        #endregion

        #region IProvider Members

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransform; } 
            set
            {
                if (_coordinateTransform != value)
                {
                    _coordinateTransform = value;
                    OnPropertyChanged(CoordinateTransformationProperty);
                }
            }
        }

        public abstract ICoordinateSystem SpatialReference { get; }

        public virtual Boolean IsOpen
        {
            get { return _isOpen; }
            private set
            {
                _isOpen = value;
                OnPropertyChanged(IsOpenProperty);
            }
        }

        public abstract Int32? Srid { get; }

        public abstract IExtents GetExtents();

        public abstract String ConnectionId { get; }

        public virtual void Open()
        {
            IsOpen = true;
        }

        public virtual void Close()
        {
            IsOpen = false;
        }

        public abstract Object ExecuteQuery(Expression query);

        #endregion

        protected abstract PropertyDescriptorCollection GetClassProperties();

        protected virtual TValue GetPropertyValueInternal<TValue>(PropertyDescriptor property)
        {
            checkGetValueType<TValue>(property);

            return (TValue)GetPropertyValueInternal(property);
        }

        protected virtual Object GetPropertyValueInternal(PropertyDescriptor property)
        {
            PropertyDescriptorCollection classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                GetObjectProperty(propertyName);
            }

            if (_instanceProperties.Contains(property))
            {
                Object value;

                return _propertyValues != null && _propertyValues.TryGetValue(property, out value)
                           ? value
                           : null;
            }

            throw new InvalidOperationException("Property doesn't exist in this instance: " +
                                                property.Name);
        }

        protected virtual void SetPropertyValueInternal<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);

            SetPropertyValueInternal(property, (Object)value);
        }

        protected virtual void SetPropertyValueInternal(PropertyDescriptor property, Object value)
        {
            PropertyDescriptorCollection classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                SetObjectProperty(propertyName, value);
            }
            else if (_instanceProperties.Contains(property))
            {
                if (_propertyValues == null)
                {
                    _propertyValues = new Dictionary<PropertyDescriptor, Object>();
                }

                _propertyValues[property] = value;
            }
            else
            {
                throw new InvalidOperationException("Property doesn't exist on this layer: " +
                                                    property.Name);
            }
        }

        protected virtual void SetObjectProperty(String propertyName, Object value)
        {
            if (propertyName.Equals(ConnectionIdProperty.Name) ||
                propertyName.Equals(IsOpenProperty.Name) ||
                propertyName.Equals(SridProperty.Name) ||
                propertyName.Equals(SpatialReferenceProperty.Name))
            {
                throw new InvalidOperationException("The property '" + propertyName + "' is read-only.");
            }

            if (propertyName.Equals(CoordinateTransformation.Name))
            {
                CoordinateTransformation = value as ICoordinateTransformation;
            }
        }

        protected virtual Object GetObjectProperty(String propertyName)
        {
            if (propertyName.Equals(CoordinateTransformation.Name))
            {
                return CoordinateTransformation;
            }

            if (propertyName.Equals(ConnectionIdProperty.Name))
            {
                return ConnectionId;
            }

            if (propertyName.Equals(IsOpenProperty.Name))
            {
                return IsOpen;
            }

            if (propertyName.Equals(SridProperty.Name))
            {
                return Srid;
            }

            if (propertyName.Equals(SpatialReferenceProperty.Name))
            {
                return SpatialReference;
            }

            return null;
        }

        protected void OnPropertyChanged(PropertyDescriptor property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property.Name));
            }
        }

        #region Private helper methods
        private static void checkGetValueType<TValue>(PropertyDescriptor property)
        {
            if (!typeof(TValue).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("The type of the property isn't " +
                                            "assignable to the value variable.");
            }
        }

        private static void checkSetValueType<TValue>(PropertyDescriptor property)
        {
            if (!property.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException("The type of the value isn't " +
                                            "assignable to the property.");
            }
        }

        private void ensureInstanceProperties()
        {
            if (_instanceProperties != null)
            {
                return;
            }

            PropertyDescriptorCollection classProperties = GetClassProperties();
            PropertyDescriptor[] propArray = new PropertyDescriptor[classProperties.Count];
            classProperties.CopyTo(propArray, 0);
            _instanceProperties = new PropertyDescriptorCollection(propArray, false);
        }

        private void checkPropertyParameter(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            if (!HasProperty(property))
            {
                throw new InvalidOperationException("Property doesn't exist for instance " +
                                                    ToString());
            }

            if (property.IsReadOnly)
            {
                throw new InvalidOperationException(String.Format("Property {0} is read only.",
                                                                  property.Name));
            }
        }

        #endregion
    }
}
