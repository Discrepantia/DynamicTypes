﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypes
{

    public class iPropertyGenerator<T> : PropertyGenerator
    {
        public iPropertyGenerator(string name) : base(name, null)
        {
            OverrideDefinition = typeof(T);
            var p = typeof(T).GetProperty(name);

            Type = p.PropertyType;
            BackingField = new FieldGenerator("m_" + name, Type);

            Get = p.GetMethod != null;
            Set = p.GetMethod != null;
        }
    }
    public class PropertyGenerator<T> : PropertyGenerator
    {
        public PropertyGenerator(string name) : base(name, typeof(T))
        {
        }
    }

    /// <summary>
    /// A Simple generator for Flat Properties eg. get; set;
    /// </summary>
    [DebuggerDisplay("PropertyGenerator {PropertyName} {Type.FullName}")]
    public class PropertyGenerator : MemberGenerator
    {

        #region Properties

        /// <summary>
        /// Name of the Property
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// The FieldGenerator of this class
        /// </summary>
        public FieldGenerator BackingField { get; set; }

        /// <summary>
        /// The PropertyBuilder of this class
        /// </summary>
        protected internal PropertyBuilder internalProperty { get; set; }

        /// <summary>
        /// The Property that is Generated (Only available after Compiling) 
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// The Field that is Generated (Only available after Compiling) 
        /// </summary>
        public FieldInfo Field { get; set; }

        /// <summary>
        /// Defines if a getProperty will be defined
        /// </summary>
        public bool Get { get; set; } = true;
        /// <summary>
        /// Defines if a setProperty will be defined
        /// </summary>
        public bool Set { get; set; } = true;


        #endregion

        #region Constructors

        public PropertyGenerator()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="PropertyGenerator"/>
        /// </summary>
        /// <param name="name">Name of the Property</param>
        /// <param name="type">Type of the Property</param>
        public PropertyGenerator(string name, Type type) : base(type)
        {
            PropertyName = name;
            BackingField = new FieldGenerator("m_" + name, type);
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void DefineMember(TypeBuilder tb)
        {
            BackingField.DefineMember(tb);

            internalProperty = tb.DefineProperty(PropertyName, PropertyAttributes.HasDefault, Type, null);

            foreach (var item in Attributes)
            {
                internalProperty.SetCustomAttribute(item.AttributeBuilder);
            }

            var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            if(OverrideDefinition != null)
            {
                getSetAttr = getSetAttr | MethodAttributes.Virtual;
            }
            if(Get)
            {
                var mbGet = tb.DefineMethod("get_" + PropertyName, getSetAttr, Type, Type.EmptyTypes);
                var getIL = mbGet.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, BackingField.internalField);
                getIL.Emit(OpCodes.Ret);
                internalProperty.SetGetMethod(mbGet);

                if (OverrideDefinition != null)
                {
                    tb.DefineMethodOverride(mbGet, OverrideDefinition.GetMethod("get_" + PropertyName));
                }
            }
            if(Set)
            {
                var mbSet = tb.DefineMethod("set_" + PropertyName, getSetAttr, null, new Type[] { Type });
                var setIL = mbSet.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, BackingField.internalField);
                setIL.Emit(OpCodes.Ret);
                internalProperty.SetSetMethod(mbSet);

                if (OverrideDefinition != null)
                {
                    tb.DefineMethodOverride(mbSet, OverrideDefinition.GetMethod("set_" + PropertyName));
                }
            }

        }


        public override void Compiled(TypeGenerator cg)
        {
            Property = cg.Type.GetProperty(PropertyName);
            Field = cg.Type.GetField(BackingField.FieldName);
            base.Compiled(cg);
        }
        #endregion

    }
}