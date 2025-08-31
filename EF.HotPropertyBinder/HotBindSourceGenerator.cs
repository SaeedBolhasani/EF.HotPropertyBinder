using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace EF.HotPropertyBinder
{
    [Generator]
    public class HotBindSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Add a debug source to verify the generator is running
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "DebugInfo.g.cs", 
                SourceText.From("// HotBind Source Generator is running\n", Encoding.UTF8)));

            // Find classes with properties that have HotBindAttribute
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsClassWithProperties(s),
                    transform: static (ctx, _) => GetClassWithHotBindProperties(ctx))
                .Where(static m => m is not null);

            // Combine with compilation
            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            // Generate the hot bind helper
            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Left, source.Right, spc));
        }

        private static bool IsClassWithProperties(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax { Members: var members } &&
                   members.OfType<PropertyDeclarationSyntax>().Any();
        }

        private static ClassInfo? GetClassWithHotBindProperties(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
            
            if (classSymbol == null)
                return null;

            var hotBindProperties = new List<PropertyInfo>();

            foreach (var member in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
            {
                var propertySymbol = context.SemanticModel.GetDeclaredSymbol(member);
                if (propertySymbol == null) continue;

                // Check if property has HotBindAttribute (with more flexible matching)
                var hasHotBindAttribute = propertySymbol.GetAttributes()
                    .Any(attr => 
                        attr.AttributeClass?.Name == "HotBindAttribute" || 
                        attr.AttributeClass?.Name == "HotBind" ||
                        attr.AttributeClass?.ToDisplayString().Contains("HotBind"));

                if (hasHotBindAttribute)
                {
                    hotBindProperties.Add(new PropertyInfo(
                        propertySymbol.Name,
                        propertySymbol.Type.ToDisplayString()));
                }
            }

            if (hotBindProperties.Count == 0)
                return null;

            return new ClassInfo(
                classSymbol.Name,
                classSymbol.ContainingNamespace.ToDisplayString(),
                hotBindProperties);
        }

        private static void Execute(Compilation compilation, ImmutableArray<ClassInfo?> classes, SourceProductionContext context)
        {
            var validClasses = classes.Where(c => c != null).Cast<ClassInfo>().ToList();
            
            // Define the HOTBIND_GENERATED symbol to enable conditional compilation
            context.AddSource("HotBindConstants.g.cs", SourceText.From(
                "#define HOTBIND_GENERATED\n// This file defines the HOTBIND_GENERATED symbol for conditional compilation", 
                Encoding.UTF8));
            
            // Always generate the helper class, even if no classes found
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("namespace EF.HotPropertyBinder.Generated");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("    public static class HotBindHelper");
            sourceBuilder.AppendLine("    {");
            sourceBuilder.AppendLine("        public static void BindHotProperties(object entity, IServiceProvider serviceProvider)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            switch (entity)");
            sourceBuilder.AppendLine("            {");

            if (validClasses.Count > 0)
            {
                foreach (var classInfo in validClasses)
                {
                    GenerateClassCase(sourceBuilder, classInfo);
                }
            }
            else
            {
                sourceBuilder.AppendLine("                // No classes with HotBindAttribute properties found");
            }

            sourceBuilder.AppendLine("                default:");
            sourceBuilder.AppendLine("                    break;");
            sourceBuilder.AppendLine("            }");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            context.AddSource("HotBindHelper.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private static void GenerateClassCase(StringBuilder sourceBuilder, ClassInfo classInfo)
        {
            var fullTypeName = string.IsNullOrEmpty(classInfo.Namespace) 
                ? classInfo.ClassName 
                : $"{classInfo.Namespace}.{classInfo.ClassName}";

            sourceBuilder.AppendLine($"                case {fullTypeName} {classInfo.ClassName.ToLowerInvariant()}Entity:");

            foreach (var property in classInfo.Properties)
            {
                sourceBuilder.AppendLine($"                    {classInfo.ClassName.ToLowerInvariant()}Entity.{property.Name} = ({property.TypeName})serviceProvider.GetRequiredService<{property.TypeName}>();");
            }

            sourceBuilder.AppendLine("                    break;");
            sourceBuilder.AppendLine();
        }
    }

    public record ClassInfo(string ClassName, string Namespace, List<PropertyInfo> Properties);
    public record PropertyInfo(string Name, string TypeName);
}