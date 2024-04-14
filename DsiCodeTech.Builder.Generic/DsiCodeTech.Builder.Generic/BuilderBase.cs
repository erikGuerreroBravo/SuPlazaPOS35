using AutoFixture;
using AutoFixture.Dsl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Builder.Generic
{
    /// <summary>
    /// Provides the base implementation of a builder.
    /// </summary>
    public abstract class BuilderBase<TObject,TBuilder> where TBuilder: BuilderBase<TObject, TBuilder>
    {
        /// <summary>
        /// Inicializamos la instancia de la clase
        /// </summary>
        protected BuilderBase() : this(CreateComposer()) {
        }

        /// <summary>
        /// Inicializacion de una nueva instancia de la clase
        /// </summary>
        /// <param name="factory"></param>
        protected BuilderBase(Func<TObject> factory) : this(CreateComposer().FromFactory(factory))
        {
            
        }
        protected BuilderBase(Fixture fixture) : this(CreateComposer(fixture))
        { 
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BuilderBase{TObject, TBuilder}"/>
        /// </summary>
        /// <param name="composer"></param>
        protected BuilderBase(IPostprocessComposer<TObject>? composer = null)
       => this.Composer = composer ?? CreateComposer();

        private IPostprocessComposer<TObject> Composer { get; set; }

        private TBuilder Builder => (TBuilder)this;

        /// <summary>
        /// Este metodo se encarga de realizar o especificar las acciones a realizar por la instancia
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TBuilder Do(Action<TObject> action)
        {
            this.Composer = this.Composer.Do(action);
            return this.Builder;
        }

        public TBuilder OmitAutoProperties()
        {
            this.Composer = this.Composer.OmitAutoProperties();
            return this.Builder;
        }
        public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker)
        {
            this.Composer = this.Composer.With(propertyPicker);
            return this.Builder;
        }
        public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, TProperty value)
        {
            this.Composer = this.Composer.With(propertyPicker, value);
            return this.Builder;
        }
        public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, Func<TProperty> valueFactory)
        {
            this.Composer = this.Composer.With(propertyPicker, valueFactory);
            return this.Builder;
        }
        public TBuilder With<TProperty, TInput>(Expression<Func<TObject, TProperty>> propertyPicker, Func<TInput, TProperty> valueFactory)
        {
            this.Composer = this.Composer.With(propertyPicker, valueFactory);
            return this.Builder;
        }
        public TBuilder WithPrivate<TProperty, TValue>(Expression<Func<TObject, TProperty>> propertyPicker, TValue value)
        {
            
            var expressionParts = propertyPicker.ToString().Split('.').Skip(1).ToList();
            if (!expressionParts.Any())
            {
                throw new ArgumentException("The expression must specify a property or field.", nameof(propertyPicker));
            }

            return this.WithPrivate(expressionParts.First(), value);
        }
        public TBuilder WithPrivate<TValue>(string propertyName, TValue value)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var propertyParts = propertyName.Split('.');
            if (propertyParts.Length > 1)
            {
                throw new ArgumentException("The expression must not contain access to a nested property or field.", nameof(propertyName));
            }

            this.Composer = this.Composer.Do(x =>
            {
                
                var flags = BindingFlags.Instance |
                            BindingFlags.SetField |
                            BindingFlags.SetProperty |
                            BindingFlags.NonPublic |
                            BindingFlags.Public;

                _ = typeof(TObject).InvokeMember(propertyName, flags, null, x, new object?[] { value }, CultureInfo.InvariantCulture);
            });
            return this.Builder;
        }
        public TBuilder WithAutoProperties()
        {
            this.Composer = this.Composer.WithAutoProperties();
            return this.Builder;
        }
        public TBuilder Without<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker)
        {
            this.Composer = this.Composer.Without(propertyPicker);
            return this.Builder;
        }
        public TObject Create() => this.Composer.Create();
        public IEnumerable<TObject> CreateMany() => this.Composer.CreateMany();
        public IEnumerable<TObject> CreateMany(int count) => this.Composer.CreateMany(count);
        private static Fixture CreateFixture() => new()
        {
            OmitAutoProperties = true,
        };
        private static ICustomizationComposer<TObject> CreateComposer(Fixture? fixture = null)
        {
            fixture ??= CreateFixture();
            return fixture.Build<TObject>();
        }
    }
}
