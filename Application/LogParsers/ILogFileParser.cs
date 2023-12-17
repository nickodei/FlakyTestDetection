using Application.LogParsers.Models;

namespace Application.LogParsers;

public interface ILogFileParser
{
    TestFile ParseLogFile(StreamReader stream);
}