using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EverlandApi.Extensions
{
    public static class StringExtensions
    {
        private const char ESCAPE_TOKEN = '\\';
        private const char START_IDENTIFIER_TOKEN = '{';
        private const char END_IDENTIFIER_TOKEN = '}';

        private static Regex IdentifierTest = new Regex("[_a-zA-Z][_a-zA-Z0-9]*");

        private enum ParserState
        {
            Regular,
            Escaped,
            Identifier
        }

        private static bool IsValidIdentifier(string identifier)
        {
            return IdentifierTest.IsMatch(identifier);
        }
        
        private static string GetValue(
            string identifier, IDictionary parameters)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (identifier == string.Empty)
                throw new ArgumentException(
                    "Format identifier cannot be empty.",
                    nameof(identifier)
                );

            object currentValue = null;
            string[] propertyNames = identifier.Split('.');
            foreach (string propertyName in propertyNames)
            {
                if (!IsValidIdentifier(propertyName))
                    throw new ArgumentException(
                        $"Invalid format identifier: \"{identifier}\" - specifically \"{propertyName}\"",
                        nameof(identifier)
                    );

                if (currentValue == null)
                    currentValue = parameters[propertyName];
                else
                {
                    var properties = currentValue.GetType().GetProperties();
                    var property = properties.FirstOrDefault(p => p.Name == propertyName);
                    if (property == null)
                        throw new KeyNotFoundException(
                            $"Could not find property: \"{propertyName}\""
                        );
                    currentValue = property.GetValue(currentValue);
                }
            }

            return currentValue.ToString();
        }

        public static string Format(
            this string formatString, IDictionary parameters)
        {
            var result = string.Empty;
            var identifier = string.Empty;
            var state = ParserState.Regular;

            foreach (char c in formatString)
            {
                switch (state)
                {
                    case ParserState.Regular:
                        switch (c)
                        {
                            case ESCAPE_TOKEN:
                                state = ParserState.Escaped;
                                break;
                            case START_IDENTIFIER_TOKEN:
                                state = ParserState.Identifier;
                                break;
                            default:
                                result += c;
                                break;
                        }
                        break;

                    case ParserState.Escaped:
                        result += c;
                        state = ParserState.Regular;
                        break;

                    case ParserState.Identifier:
                        if (c == END_IDENTIFIER_TOKEN)
                        {
                            result += GetValue(identifier, parameters);
                            identifier = string.Empty;
                            state = ParserState.Regular;
                        }
                        else
                            identifier += c;
                        break;
                }
            }

            return result;
        }
    }
}
