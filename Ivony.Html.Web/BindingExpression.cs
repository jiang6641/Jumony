﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Web
{
  public class BindingExpression
  {

    public static readonly string ExpressionArgumentPattern = @"(?<args>(?<name>\w+)(=(?<value>[^\,]+))?)";
    public static readonly string BindingExpressionPattern = string.Format( @"(?<expression>\{{(?<ename>\w+)\s*({0}(\,\s*{0})*)?\}})", ExpressionArgumentPattern );


    private static readonly Regex bindingExpressionRegex = new Regex( "^" + BindingExpressionPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled );
    private   string _name;
    private   IDictionary<string,string> _args;


    private BindingExpression( string name, IDictionary<string, string> args )
    {
      _name = name;
      _args = args;
    }

    public string Name
    {
      get { return _name; }
    }

    public IDictionary<string, string> Arguments
    {
      get { return new Dictionary<string, string>( _args ); }
    }


    public BindingExpression( IHtmlElement element ) : this( element.Name, element.Attributes().ToDictionary( a => a.Name, a => a.AttributeValue ) ) { }




    public static BindingExpression ParseExpression( IHtmlAttribute attribute )
    {
      var expression = attribute.Value();

      if ( expression == null )
        return null;

      var match = bindingExpressionRegex.Match( expression );
      if ( match == null || !match.Success )
        return null;

      return ParseExpression( match );
    }



    private static BindingExpression ParseExpression( Match match )
    {

      var _name = match.Groups["ename"].Value;

      var args = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["args"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).FirstOrDefault().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).FirstOrDefault().Value;

        args[name] = value;
      }

      return new BindingExpression( _name, args );

    }


  }
}
