﻿/*

Copyright Robert Vesse 2009-10
rvesse@vdesign-studios.com

------------------------------------------------------------------------

This file is part of dotNetRDF.

dotNetRDF is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

dotNetRDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with dotNetRDF.  If not, see <http://www.gnu.org/licenses/>.

------------------------------------------------------------------------

dotNetRDF may alternatively be used under the LGPL or MIT License

http://www.gnu.org/licenses/lgpl.html
http://www.opensource.org/licenses/mit-license.php

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if !NO_WEB
using System.Web.UI;
#endif
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing.Contexts;

//REQ:  Implement a HtmlWriter which prints a human readble Schema style document

namespace VDS.RDF.Writing
{
    public class HtmlSchemaWriter : IRdfWriter, IHtmlWriter
    {
        #region IHtmlWriter Members

        private String _stylesheet = String.Empty;
        private String _uriClass = "uri",
                       _bnodeClass = "bnode",
                       _literalClass = "literal",
                       _datatypeClass = "datatype",
                       _langClass = "langspec";
        private String _uriPrefix = String.Empty;


        /// <summary>
        /// Gets/Sets a path to a Stylesheet which is used to format the Graph output
        /// </summary>
        public string Stylesheet
        {
            get
            {
                return this._stylesheet;
            }
            set
            {
                this._stylesheet = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the anchor tags used to display the URIs of URI Nodes
        /// </summary>
        public string CssClassUri
        {
            get
            {
                return this._uriClass;
            }
            set
            {
                this._uriClass = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Blank Node IDs
        /// </summary>
        public string CssClassBlankNode
        {
            get
            {
                return this._bnodeClass;
            }
            set
            {
                this._bnodeClass = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Literals
        /// </summary>
        public string CssClassLiteral
        {
            get
            {
                return this._literalClass;
            }
            set
            {
                this._literalClass = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the anchor tags used to display Literal datatypes
        /// </summary>
        public string CssClassDatatype
        {
            get
            {
                return this._datatypeClass;
            }
            set
            {
                this._datatypeClass = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Literal language specifiers
        /// </summary>
        public string CssClassLangSpec
        {
            get
            {
                return this._langClass;
            }
            set
            {
                this._langClass = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Prefix applied to href attributes
        /// </summary>
        public String UriPrefix
        {
            get
            {
                return this._uriPrefix;
            }
            set
            {
                this._uriPrefix = value;
            }
        }

        #endregion

        /// <summary>
        /// Saves the Graph to the given File as an XHTML Table with embedded RDFa
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="filename">File to save to</param>
        public void Save(IGraph g, String filename)
        {
            StreamWriter output = new StreamWriter(filename, false, new UTF8Encoding(Options.UseBomForUtf8));
            this.Save(g, output);
        }

        /// <summary>
        /// Saves the Result Set to the given Stream as an XHTML Table with embedded RDFa
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="output">Stream to save to</param>
        public void Save(IGraph g, TextWriter output)
        {
            try
            {
                HtmlWriterContext context = new HtmlWriterContext(g, output);
                this.GenerateOutput(context);
                output.Close();
            }
            catch
            {
                try
                {
                    output.Close();
                }
                catch
                {
                    //No Catch Actions
                }
                throw;
            }
        }

        /// <summary>
        /// Internal method which generates the HTML Output for the Graph
        /// </summary>
        /// <param name="context">Writer Context</param>
        private void GenerateOutput(HtmlWriterContext context)
        {
            //Add the Namespaces we want to use later on
            context.QNameMapper.AddNamespace("owl", new Uri(NamespaceMapper.OWL));
            context.QNameMapper.AddNamespace("rdf", new Uri(NamespaceMapper.RDF));
            context.QNameMapper.AddNamespace("rdfs", new Uri(NamespaceMapper.RDFS));
            context.QNameMapper.AddNamespace("dc", new Uri("http://purl.org/dc/elements/1.1/"));
            context.QNameMapper.AddNamespace("dct", new Uri("http://purl.org/dc/terms/"));
            context.QNameMapper.AddNamespace("vann", new Uri("http://purl.org/vocab/vann/"));
            context.QNameMapper.AddNamespace("vs", new Uri("http://www.w3.org/2003/06/sw-vocab-status/ns#"));

            //Find the Node that represents the Schema Ontology
            //Assumes there is exactly one thing given rdf:type owl:Ontology
            UriNode ontology = context.Graph.CreateUriNode(NamespaceMapper.OWL + "Ontology");
            UriNode rdfType = context.Graph.CreateUriNode(RdfSpecsHelper.RdfType);
            UriNode rdfsLabel = context.Graph.CreateUriNode(NamespaceMapper.RDFS + "label");
            INode ontoNode = context.Graph.GetTriplesWithPredicateObject(rdfType, ontology).Select(t => t.Subject).FirstOrDefault();
            INode ontoLabel = (ontoNode != null) ? context.Graph.GetTriplesWithSubjectPredicate(ontoNode, rdfsLabel).Select(t => t.Object).FirstOrDefault() : null;

            //Page Header
            context.HtmlWriter.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML+RDFa 1.0//EN\" \"http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd\">");
            context.HtmlWriter.WriteLine();
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Head);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Title);
            context.HtmlWriter.WriteEncodedText("Schema");
            if (ontoNode != null && ontoLabel != null)
            {
                context.HtmlWriter.WriteEncodedText(" - " + ontoLabel.ToSafeString());
            }
            else if (context.Graph.BaseUri != null)
            {
                context.HtmlWriter.WriteEncodedText(" - " + context.Graph.BaseUri.ToString());
            }
            context.HtmlWriter.RenderEndTag();
            if (!this._stylesheet.Equals(String.Empty))
            {
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, this._stylesheet);
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Link);
                context.HtmlWriter.RenderEndTag();
            }
            context.HtmlWriter.RenderEndTag();
#if !NO_WEB
            context.HtmlWriter.WriteLine();
#endif

            //Start Body
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

            //Title
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
            context.HtmlWriter.WriteEncodedText("Schema");
            if (ontoNode != null && ontoLabel != null)
            {
                context.HtmlWriter.WriteEncodedText(" - " + ontoLabel.ToSafeString());
            }
            else if (context.Graph.BaseUri != null)
            {
                context.HtmlWriter.WriteEncodedText(" - " + context.Graph.BaseUri.ToString());
            }
            context.HtmlWriter.RenderEndTag();
#if !NO_WEB
            context.HtmlWriter.WriteLine();
#endif

            //Show the Description of the Schema (if any)
            if (ontoNode != null)
            {
                SparqlParameterizedString getOntoDescrip = new SparqlParameterizedString();
                getOntoDescrip.Namespaces = context.QNameMapper;
                getOntoDescrip.QueryText = "SELECT * WHERE { @onto a owl:Ontology . OPTIONAL { @onto rdfs:comment ?description } . OPTIONAL { @onto vann:preferredNamespacePrefix ?nsPrefix ; vann:preferredNamespaceUri ?nsUri } . OPTIONAL { @onto dc:creator ?creator . ?creator (foaf:name | rdfs:label) ?creatorName } }";

                //TODO: Show the Namespace information for the Schema
            }

            //TODO: Show lists of all Classes and Properties in the Schema

            //TODO: Show details for each class

            //TODO: Show details for each property

            //End of Page
            context.HtmlWriter.RenderEndTag(); //End Body
            context.HtmlWriter.RenderEndTag(); //End Html
        }

        /// <summary>
        /// Generates Output for a given Node
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="n">Node</param>
        private void GenerateNodeOutput(HtmlWriterContext context, INode n)
        {
            this.GenerateNodeOutput(context, n, null);
        }

        /// <summary>
        /// Generates Output for a given Node
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="n">Node</param>
        /// <param name="t">Triple being written</param>
        private void GenerateNodeOutput(HtmlWriterContext context, INode n, Triple t)
        {
            //Embed RDFa on the Node Output
            bool rdfASerializable = false;
            if (t != null)
            {
                if (t.Predicate.NodeType == NodeType.Uri)
                {
                    //Use @about to specify the Subject
                    if (t.Subject.NodeType == NodeType.Uri)
                    {
                        rdfASerializable = true;
                        context.HtmlWriter.AddAttribute("about", context.UriFormatter.FormatUri(t.Subject.ToString()));
                    }
                    else if (t.Subject.NodeType == NodeType.Blank)
                    {
                        rdfASerializable = true;
                        context.HtmlWriter.AddAttribute("about", "[" + t.Subject.ToString() + "]");
                    }
                    else
                    {
                        this.RaiseWarning("Cannot serialize a Triple since the Subject is not a URI/Blank Node: " + t.Subject.ToString());
                    }

                    //Then if we can serialize this Triple we serialize the Predicate
                    if (rdfASerializable)
                    {
                        //Get the CURIE for the Predicate
                        String curie;
                        String tempNamespace;
                        if (context.QNameMapper.ReduceToQName(t.Predicate.ToString(), out curie, out tempNamespace))
                        {
                            //Extract the Namespace and make sure it's registered on this Attribute
                            String ns = curie.Substring(0, curie.IndexOf(':'));
                            context.HtmlWriter.AddAttribute("xmlns:" + ns, context.UriFormatter.FormatUri(context.QNameMapper.GetNamespaceUri(ns)));
                        }
                        else
                        {
                            this.RaiseWarning("Cannot serialize a Triple since the Predicate cannot be reduced to a QName: " + t.Predicate.ToString());
                            rdfASerializable = false;
                        }

                        if (rdfASerializable)
                        {
                            switch (t.Object.NodeType)
                            {
                                case NodeType.Blank:
                                case NodeType.Uri:
                                    //If the Object is a URI or a Blank then we specify the predicate with @rel
                                    context.HtmlWriter.AddAttribute("rel", curie);
                                    break;

                                case NodeType.Literal:
                                    //If the Object is a Literal we specify the predicate with @property
                                    context.HtmlWriter.AddAttribute("property", curie);
                                    break;
                                default:
                                    this.RaiseWarning("Cannot serialize a Triple since the Object is not a URI/Blank/Literal Node: " + t.Object.ToString());
                                    rdfASerializable = false;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    this.RaiseWarning("Cannot serialize a Triple since the Predicate is not a URI Node: " + t.Predicate.ToString());
                }
            }

            String qname;
            switch (n.NodeType)
            {
                case NodeType.Blank:
                    if (rdfASerializable)
                    {
                        //Need to embed the CURIE for the BNode in the @resource attribute
                        context.HtmlWriter.AddAttribute("resource", "[" + n.ToString() + "]");
                    }

                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._bnodeClass);
                    context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                    context.HtmlWriter.WriteEncodedText(n.ToString());
                    context.HtmlWriter.RenderEndTag();
                    break;

                case NodeType.Literal:
                    LiteralNode lit = (LiteralNode)n;
                    if (lit.DataType != null)
                    {
                        if (rdfASerializable)
                        {
                            //Need to embed the datatype in the @datatype attribute
                            String dtcurie, dtnamespace;
                            if (context.QNameMapper.ReduceToQName(lit.DataType.ToString(), out dtcurie, out dtnamespace))
                            {
                                //Extract the Namespace and make sure it's registered on this Attribute
                                String ns = dtcurie.Substring(0, dtcurie.IndexOf(':'));
                                context.HtmlWriter.AddAttribute("xmlns:" + ns, context.UriFormatter.FormatUri(context.QNameMapper.GetNamespaceUri(ns)));
                                context.HtmlWriter.AddAttribute("datatype", dtcurie);
                            }
                        }

                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._literalClass);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                        if (lit.DataType.ToString().Equals(Parsing.RdfSpecsHelper.RdfXmlLiteral))
                        {
                            context.HtmlWriter.Write(lit.Value);
                        }
                        else
                        {
                            context.HtmlWriter.WriteEncodedText(lit.Value);
                        }
                        context.HtmlWriter.RenderEndTag();

                        //Output the Datatype
                        context.HtmlWriter.WriteEncodedText("^^");
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, lit.DataType.ToString());
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._datatypeClass);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                        if (context.QNameMapper.ReduceToQName(lit.DataType.ToString(), out qname))
                        {
                            context.HtmlWriter.WriteEncodedText(qname);
                        }
                        else
                        {
                            context.HtmlWriter.WriteEncodedText(lit.DataType.ToString());
                        }
                        context.HtmlWriter.RenderEndTag();
                    }
                    else
                    {
                        if (rdfASerializable)
                        {
                            if (!lit.Language.Equals(String.Empty))
                            {
                                //Need to add the language as an xml:lang attribute
                                context.HtmlWriter.AddAttribute("xml:lang", lit.Language);
                            }
                        }
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._literalClass);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                        context.HtmlWriter.WriteEncodedText(lit.Value);
                        context.HtmlWriter.RenderEndTag();
                        if (!lit.Language.Equals(String.Empty))
                        {
                            context.HtmlWriter.WriteEncodedText("@");
                            context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._langClass);
                            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                            context.HtmlWriter.WriteEncodedText(lit.Language);
                            context.HtmlWriter.RenderEndTag();
                        }
                    }
                    break;

                case NodeType.GraphLiteral:
                    //Error
                    throw new RdfOutputException(WriterErrorMessages.GraphLiteralsUnserializable("HTML"));

                case NodeType.Uri:
                    if (rdfASerializable && !this._uriPrefix.Equals(String.Empty))
                    {
                        //If the URIs are being prefixed with something then we need to set the original
                        //URI in the resource attribute to generate the correct triple
                        context.HtmlWriter.AddAttribute("resource", n.ToString());
                    }

                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this._uriClass);
                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, this._uriPrefix + n.ToString());
                    context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    if (context.QNameMapper.ReduceToQName(n.ToString(), out qname))
                    {
                        context.HtmlWriter.WriteEncodedText(qname);
                    }
                    else
                    {
                        context.HtmlWriter.WriteEncodedText(n.ToString());
                    }
                    context.HtmlWriter.RenderEndTag();
                    break;

                default:
                    throw new RdfOutputException(WriterErrorMessages.UnknownNodeTypeUnserializable("HTML"));
            }
        }

        /// <summary>
        /// Helper method for raising the <see cref="Warning">Warning</see> event
        /// </summary>
        /// <param name="message">Warning Message</param>
        private void RaiseWarning(String message)
        {
            RdfWriterWarning d = this.Warning;
            if (d != null)
            {
                d(message);
            }
        }

        /// <summary>
        /// Event which is raised if there is a non-fatal error with the RDF being output
        /// </summary>
        public event RdfWriterWarning Warning;
    }
}
