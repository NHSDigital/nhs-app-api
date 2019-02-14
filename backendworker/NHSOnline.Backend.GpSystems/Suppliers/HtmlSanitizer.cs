using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;

namespace NHSOnline.Backend.GpSystems.Suppliers
{
    public class HtmlSanitizer: IHtmlSanitizer
    {
        private readonly HashSet<string> _blackList = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            { "script" },
            { "iframe" },
            { "form" },
            { "object" },
            { "embed" },
            { "link" },
            { "meta" }
        };

        public string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var doc = new HtmlDocument
            {
                OptionWriteEmptyNodes = true,
            };

            html = RemoveEmptyAnchorTags(html);
            
            doc.LoadHtml(html);
            SanitizeHtmlNode(doc.DocumentNode, doc);

            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw))
            {
                doc.DocumentNode.WriteTo(writer);
                var output = sw.ToString();

                if (string.IsNullOrEmpty(output))
                {
                    return output;
                }
                
                var at = output.IndexOf("?>", StringComparison.Ordinal);
                output = output.Substring(at + 2);

                return output;
            }
        }

        private static string RemoveEmptyAnchorTags(string html)
        {
            return Regex.Replace(html, @"<a[^>]*>\s*<\/[^>]*a>", string.Empty);
        }
        
        private void SanitizeHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (SanitizeBlackListedNode(node))
                {
                    return;
                }

                if (SanitizeStyleNode(node))
                {
                    return;
                }

                SanitizeAttributes(node);
            }

            if (!node.HasChildNodes)
            {
                return;   
            }
            
            for (var i = node.ChildNodes.Count - 1; i >= 0; i--)
            {
                SanitizeHtmlNode(node.ChildNodes[i], doc);
            }
        }

        private static bool HasScriptLinks(string value)
        {
            return value.Contains("javascript:", StringComparison.OrdinalIgnoreCase) || 
                   value.Contains("vbscript:",  StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasExpressionLinks(string value)
        {
            return value.Contains("expression", StringComparison.OrdinalIgnoreCase);
        }
        
        private bool SanitizeBlackListedNode(HtmlNode node)
        {
            if (!_blackList.Contains(node.Name))
            {
                return false;
            }
            
            node.Remove();
            return true;
        }

        private static bool SanitizeStyleNode(HtmlNode node)
        {
            if (!string.Equals(node.Name, "style", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrEmpty(node.InnerHtml))
            {
                return false;
            }
            
            if (HasExpressionLinks(node.InnerHtml) || HasScriptLinks(node.InnerHtml))
            {
                node.ParentNode.RemoveChild(node);
                return true;
            }
            else
            {
                SanitizeUrlsInStyleNode(node);
                return false;
            }
        }

        private static void SanitizeUrlsInStyleNode(HtmlNode node)
        {
            var sanitizedStyleHtml = (Regex.Replace(node.InnerHtml, @"(url\(.*\))", "none"));
            node.InnerHtml = sanitizedStyleHtml;
        }
        
        private static void SanitizeAttributes(HtmlNode node)
        {
            if (!node.HasAttributes)
            {
                return;
            }
            
            for (var i = node.Attributes.Count - 1; i >= 0; i--)
            {
                var currentAttribute = node.Attributes[i];

                if (SanitizeAttributeEventHandlers(currentAttribute, node) ||
                    SanitizeAttributeCssExpressions(currentAttribute, node))
                {
                    continue;
                }

                SanitizeAttributeScriptLinks(currentAttribute, node);                                             
            }
        }

        private static bool SanitizeAttributeEventHandlers(HtmlAttribute attribute, HtmlNode node)
        {
            if (attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            {
                node.Attributes.Remove(attribute);
                return true;
            }

            return false;
        }

        private static bool SanitizeAttributeCssExpressions(HtmlAttribute attribute, HtmlNode node)
        {
            if (string.Equals(attribute.Name, "style", StringComparison.OrdinalIgnoreCase) &&
                 attribute.Value != null && (HasExpressionLinks(attribute.Value) ||
                HasScriptLinks(attribute.Value)))
            {
                node.Attributes.Remove(attribute);
                return true;
            }

            return false;
        }

        private static void SanitizeAttributeScriptLinks(HtmlAttribute attribute, HtmlNode node)
        {
            if (attribute.Value != null &&
                HasScriptLinks(attribute.Value))
            {
                node.Attributes.Remove(attribute);
            }
        }
    }
}