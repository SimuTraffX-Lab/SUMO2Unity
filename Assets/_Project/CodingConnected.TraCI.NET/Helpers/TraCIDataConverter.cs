using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodingConnected.TraCI.NET.Types;

namespace CodingConnected.TraCI.NET.Helpers
{
    internal static class TraCIDataConverter
    {
        #region Static Methods

        internal static TraCIResponse<T> ExtractDataFromResponse<T>(TraCIResult[] response, byte commandType, byte messageType = 0)
        {
            if (response?.Length > 0)
            {
                var r1 = response.FirstOrDefault(x => x.Identifier == commandType);
                if (r1?.Response[0] == 0x00) // Success
                {
                    if (r1.Identifier == TraCIConstants.CMD_SIMSTEP)
                    {
                        if (r1.Length != 5)
                        {
                            var tmp = GetDataFromSimStepResponse(r1);

                            return new TraCIResponse<T>
                            {
                                Identifier = r1.Identifier,
                                ResponseIdentifier = null,
                                Variable = null,
                                Result = ResultCode.Success,
                                Content = (T)tmp
                            };
                        }
                    }
                    // check if first byte is as requested (it gives the type of data requested)
                    var r2 = response.FirstOrDefault(x => x.Identifier == commandType + 0x10);
                    if (r2?.Response[0] == messageType)
                    {
                        // after the type of data, there is the length of the id (a string that we will skip)
                        var take = r2.Response.Skip(1).Take(4).Reverse().ToArray();
                        var idl = BitConverter.ToInt32(take, 0);
                        // after the string is the type of data returned
                        var type = r2.Response[5 + idl];
                        // now read and translate the data
                        GetValueFromTypeAndArray(type, r2.Response.Skip(6 + idl).ToArray(), out object contentAsObject);


                        return new TraCIResponse<T>
                        {
                            Identifier = r1.Identifier,
                            ResponseIdentifier = r2.Identifier,
                            Variable = r2.Response[0],
                            Result = ResultCode.Success,
                            Content = (T)contentAsObject
                        };
                    }
                    else
                    {
                        // for state changing methods without response content
                        return new TraCIResponse<T>
                        {
                            Identifier = r1.Identifier,
                            ResponseIdentifier = null,
                            Variable = null,
                            Result = ResultCode.Success,
                            Content = default
                        };
                    }
                }

                if (r1?.Response[0] == 0xFF) // Failed
                {
                    var take = r1.Response.Skip(1).Take(4).Reverse().ToArray();
                    var dlen = BitConverter.ToInt32(take, 0);
                    var sb = new StringBuilder();
                    var k1 = 5;
                    for (var j = 0; j < dlen; ++j)
                    {
                        sb.Append((char)r1.Response[k1]);
                        ++k1;
                    }

                    return new TraCIResponse<T>
                    {
                        Identifier = r1.Identifier,
                        ResponseIdentifier = null,
                        Variable = null,
                        Result = ResultCode.Failed,
                        Content = default,
                        ErrorMessage = "TraCI reports command failure: " + sb
                    };
                }

                if (r1?.Response[0] == 0x01) // Not implemented
                {
                    var take = r1.Response.Skip(1).Take(4).Reverse().ToArray();
                    var dlen = BitConverter.ToInt32(take, 0);
                    var sb = new StringBuilder();
                    var k1 = 5;
                    for (var j = 0; j < dlen; ++j)
                    {
                        sb.Append((char)r1.Response[k1]);
                        ++k1;
                    }

                    return new TraCIResponse<T>
                    {
                        Identifier = r1.Identifier,
                        ResponseIdentifier = null,
                        Variable = null,
                        Result = ResultCode.NotImplemented,
                        Content = default,
                        ErrorMessage = "TraCI reports command not implemented: " + sb
                    };
                }
            }
            return null;
        }

        private static object GetDataFromSimStepResponse(TraCIResult r1)
        {
            var returnList = new List<TraCISubscriptionResponse>();
            int offset = 5;

            // extract number of subscriptions
            var revSubCount = r1.Response.Skip(offset).Take(4).Reverse().ToArray();
            var subCount = BitConverter.ToInt32(revSubCount, 0);

            offset = 9;
            // extract the result of the subscriptions
            var subResult = r1.Response[offset++];

            // if subscription result is success
            if (subResult == 0)
            {
                for (int i = 0; i < subCount; i++)
                {
                    // extract the length of the subscription
                    var revSubLen = r1.Response.Skip(offset).Take(4).Reverse().ToArray();
                    var len = BitConverter.ToInt32(revSubLen, 0);
                    offset += 4;

                    // Subscription
                    var subResponseCode = r1.Response[offset++];

                    var low = subResponseCode & 0x0F;
                    var high = subResponseCode >> 4;

                    // 0xeX => Variable Subscription Response
                    // 0x9X => Object Context Subscription Response
                    if (high == 0x0e)
                    {
                        // extract the object id
                        offset = GetString(r1.Response, offset, out TraCIString objectId);

                        // extract the number of variables
                        var countVariables = r1.Response[offset++];

                        offset = CreateVariableSubscriptionResponse(objectId, r1, subResponseCode, countVariables, offset, out var subResponse);

                        offset++;
                        returnList.Add(subResponse);
                    }
                    else if (high == 0x09)
                    {
                        // extract the object id of the EGO object
                        offset = GetString(r1.Response, offset, out TraCIString objectId);

                        // extract the context domain the subscription happened under
                        var contextDomain = r1.Response[offset++];

                        // extract the number of variables that was returned for each object
                        var countVariables = r1.Response[offset++];

                        // extract the number of objects that are inside the context range
                        offset = GetInteger(r1.Response, offset, out var countObject);
                        var objectCount = countObject.Value;

                        var subContextResponse = new TraCIContextSubscriptionResponse(
                            objectId.Value, 
                            subResponseCode,
                            contextDomain,
                            countVariables,
                            objectCount
                            );

                        for (int objectNum = 0; objectNum < objectCount; objectNum++)
                        {
                            offset = GetString(r1.Response, offset, out var curObjectId);

                            offset = CreateVariableSubscriptionResponse(curObjectId, r1, subResponseCode, countVariables, offset, out var curSubResponse);

                            subContextResponse.VariableSubscriptionByObjectId.Add(curObjectId.Value, curSubResponse);

                        }
                        offset++;
                        returnList.Add(subContextResponse);
                    }
                }

                return returnList;
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        private static int CreateVariableSubscriptionResponse(TraCIString objectId, TraCIResult r1, byte subResponseCode, byte countVariables, int offset, out TraCIVariableSubscriptionResponse variableSubscriptionResponce)
        {
            variableSubscriptionResponce = new TraCIVariableSubscriptionResponse(objectId.Value, countVariables, subResponseCode);

            for (int varNum = 0; varNum < countVariables; varNum++)
            {
                // extract variable identifier, status and datatype of the response
                var variable = r1.Response[offset++];
                var result = r1.Response[offset++];
                var datatype = r1.Response[offset++];

                // extract value by datatype
                offset += GetValueFromTypeAndArray(datatype, r1.Response.Skip(offset), out object contentAsObject);

                switch (datatype)
                {
                    case TraCIConstants.POSITION_LON_LAT:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<LonLatPosition>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (LonLatPosition)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.POSITION_2D:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<Position2D>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (Position2D)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.POSITION_LON_LAT_ALT:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<LonLatAltPosition>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (LonLatAltPosition)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.POSITION_3D:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<Position3D>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (Position3D)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.POSITION_ROADMAP:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<RoadMapPosition>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (RoadMapPosition)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_BOUNDINGBOX:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<BoundaryBox>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (BoundaryBox)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_POLYGON:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<Polygon>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (Polygon)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_UBYTE:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<byte>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (byte)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_BYTE:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<byte>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (byte)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_INTEGER:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<int>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (int)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_FLOAT:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<float>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (float)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_DOUBLE:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<double>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (double)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_STRING:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<string>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (string)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_STRINGLIST:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<List<string>>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (List<string>)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_COLOR:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<Color>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (Color)contentAsObject
                            });
                            break;
                        }
                    case TraCIConstants.TYPE_TLPHASELIST:
                        {
                            throw new NotImplementedException("There is no handler for Traffic Light Phase List (ubyte identifier: 0x0D). Unclear definition of this datatyp. See http://sumo.dlr.de/wiki/TraCI/Protocol#Data_types");
                        }
                    case TraCIConstants.TYPE_COMPOUND:
                        {
                            variableSubscriptionResponce.responseByVariableCode.Add(
                                variable,
                            new TraCIResponse<CompoundObject>
                            {
                                Identifier = subResponseCode,
                                Variable = variable,
                                Result = (ResultCode)result,
                                Content = (CompoundObject)contentAsObject
                            });
                            break;
                        }
                    default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                }
            }

            return offset;
        }


        internal static List<EdgeInformation> ConvertToListOfEdgeInformation(CompoundObject content)
        {
            var ret = new List<EdgeInformation>();

            var numberOfEdges = (content.Value[0] as TraCIInteger).Value;

            for (int i = 0; i < numberOfEdges; i++)
            {
                var edge = new EdgeInformation();
                edge.LaneId = (content.Value[(6 * i) + 1] as TraCIString).Value;
                edge.Length = (content.Value[(6 * i) + 2] as TraCIDouble).Value;
                edge.Occupation = (content.Value[(6 * i) + 3] as TraCIDouble).Value;
                edge.OffsetToBestLane = (content.Value[(6 * i) + 4] as TraCIByte).Value;
                edge.laneInformation = (content.Value[(6 * i) + 5] as TraCIUByte).Value;
                edge.BestSubsequentLanes = (content.Value[(6 * i) + 6] as TraCIStringList).Value;
                ret.Add(edge);
            }

            return ret;
        }
        internal static List<TrafficLightSystem> ConvertToListOfTrafficLightSystem(CompoundObject content)
        {
            var ret = new List<TrafficLightSystem>();

            var numberOfTLS = (content.Value[0] as TraCIInteger).Value;

            for (int i = 0; i < numberOfTLS; i++)
            {
                var tls = new TrafficLightSystem();
                tls.TrafficLightSystemId = (content.Value[(4 * i) + 1] as TraCIString).Value;
                tls.TrafficLightSystemLinkIndex = (content.Value[(4 * i) + 2] as TraCIInteger).Value;
                tls.DistanceToTrafficLightSystem = (content.Value[(4 * i) + 3] as TraCIDouble).Value;
                tls.LinkState = (content.Value[(4 * i) + 4] as TraCIByte).Value;

                ret.Add(tls);
            }

            return ret;
        }


        internal static TrafficCompleteLightProgram ConvertToTrafficLightCompleteProgramm(CompoundObject content)
        {
            var ret = new TrafficCompleteLightProgram();

            ret.NumberOfLogics = (content.Value[0] as TraCIInteger).Value;

            int offset = 1; //offset for number of logics
            // for 1 logic  = 5 + (4 * number of phases)

            for (int i = 0; i < ret.NumberOfLogics; i++)
            {
                var tmp = new TrafficLightLogics();
                tmp.SubId = (content.Value[offset + 0] as TraCIString).Value;
                tmp.Type = (content.Value[offset + 1] as TraCIInteger).Value;
                tmp.SubParameter = (content.Value[offset + 2] as CompoundObject);
                tmp.CurrentPhaseIndex = (content.Value[offset + 3] as TraCIInteger).Value;
                tmp.NumberOfPhases = (content.Value[offset + 4] as TraCIInteger).Value;

                for (int j = 0; j < tmp.NumberOfPhases; j++)
                {
                    var phase = new TrafficLightProgramPhase();
                    phase.Duration = (content.Value[offset + 5 + (4 * j)] as TraCIInteger).Value; //4*j for the current phase
                    phase.MinDuration = (content.Value[offset + 6 + (4 * j)] as TraCIInteger).Value;
                    phase.MaxDuration = (content.Value[offset + 7 + (4 * j)] as TraCIInteger).Value;
                    phase.Definition = (content.Value[offset + 8 + (4 * j)] as TraCIString).Value;
                    tmp.TrafficLightPhases.Add(phase);
                }

                ret.TrafficLightLogics.Add(tmp);
                offset += (5 + (4 * tmp.NumberOfPhases));
            }

            return ret;
        }

        internal static ControlledLinks ConvertToControlledLinks(List<ComposedTypeBase> content)
        {
            var ret = new ControlledLinks();

            ret.NumberOfSignals = (content[0] as TraCIInteger).Value;

            for (int i = 2; i < content.Count; i += 2)
            {
                ret.Links.Add((content[i] as TraCIStringList).Value);
            }

            return ret;
        }

        internal static byte[] GetTraCIBytesFromTrafficLightPhaseList(TrafficLightPhaseList tlpl)
        {
            var bytes = new List<byte>();

            bytes.AddRange(GetTraCIBytesFromUByte((byte)tlpl.Phases.Count));

            foreach (var phase in tlpl.Phases)
            {
                bytes.AddRange(GetTraCIBytesFromASCIIString(phase.PrecRoad));
                bytes.AddRange(GetTraCIBytesFromASCIIString(phase.SuccRoad));
                bytes.AddRange(GetTraCIBytesFromUByte((byte)phase.Phase));
            }

            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromBoundaryBox(BoundaryBox bb)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromDouble(bb.LowerLeftX));
            bytes.AddRange(GetTraCIBytesFromDouble(bb.LowerLeftY));
            bytes.AddRange(GetTraCIBytesFromDouble(bb.UpperRightX));
            bytes.AddRange(GetTraCIBytesFromDouble(bb.UpperRightY));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromPolygon(Polygon p)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromUByte((byte)p.Points.Count));
            foreach (var point in p.Points)
            {
                bytes.AddRange(GetTraCIBytesFromDouble(point.X));
                bytes.AddRange(GetTraCIBytesFromDouble(point.Y));
            }

            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromLonLatAltPosition(LonLatAltPosition llap)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromDouble(llap.Longitude));
            bytes.AddRange(GetTraCIBytesFromDouble(llap.Latitude));
            bytes.AddRange(GetTraCIBytesFromDouble(llap.Altitude));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromLonLatPosition(LonLatPosition llp)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromDouble(llp.Longitude));
            bytes.AddRange(GetTraCIBytesFromDouble(llp.Latitude));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromRoadMapPosition(RoadMapPosition rmp)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromASCIIString(rmp.RoadId));
            bytes.AddRange(GetTraCIBytesFromDouble(rmp.Pos));
            bytes.AddRange(GetTraCIBytesFromUByte(rmp.LaneId));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromPosition3D(Position3D p3d)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromDouble(p3d.X));
            bytes.AddRange(GetTraCIBytesFromDouble(p3d.Y));
            bytes.AddRange(GetTraCIBytesFromDouble(p3d.Z));
            return bytes.ToArray();
        }



        internal static byte[] GetTraCIBytesFromPosition2D(Position2D p2d)
        {
            var bytes = new List<byte>();
            bytes.AddRange(GetTraCIBytesFromDouble(p2d.X));
            bytes.AddRange(GetTraCIBytesFromDouble(p2d.Y));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromFloat(float f)
        {
            return BitConverter.GetBytes(f).Reverse().ToArray();
        }

        internal static byte[] GetTraCIBytesFromUByte(byte ub)
        {
            return BitConverter.GetBytes(ub).Reverse().ToArray();
        }

        internal static byte[] GetTraCIBytesFromByte(byte b)
        {
            return BitConverter.GetBytes(b).Reverse().ToArray();
        }

        internal static byte[] GetTraCIBytesFromInt32(int i)
        {
            return BitConverter.GetBytes(i).Reverse().ToArray();
        }

        internal static byte[] GetTraCIBytesFromDouble(double d)
        {
            return BitConverter.GetBytes(d).Reverse().ToArray();
        }

        internal static byte[] GetTraCIBytesFromASCIIString(string s)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(s.Length).Reverse());
            bytes.AddRange(Encoding.ASCII.GetBytes(s));
            return bytes.ToArray();
        }

        internal static byte[] GetTraCIBytesFromASCIIStringList(List<string> los)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(los.Count).Reverse());
            foreach (var str in los)
            {
                bytes.AddRange(GetTraCIBytesFromASCIIString(str));
            }

            return bytes.ToArray();
        }

        internal static int GetValueFromTypeAndArray(byte type, IEnumerable<byte> array, out object Object)
        {
            switch (type)
            {
                case TraCIConstants.POSITION_LON_LAT:
                    {
                        var newOffset = GetPositionLonLat(array, 0, out LonLatPosition lonLatPosition);
                        Object = lonLatPosition;
                        return newOffset;
                    }
                case TraCIConstants.POSITION_2D:
                    {
                        var newOffset = Get2DPosition(array, 0, out Position2D position2D);
                        Object = position2D;
                        return newOffset;
                    }
                case TraCIConstants.POSITION_LON_LAT_ALT:
                    {
                        var newOffset = GetPositionLonLatAlt(array, 0, out LonLatAltPosition lonLatAltPosition);
                        Object = lonLatAltPosition;
                        return newOffset;
                    }
                case TraCIConstants.POSITION_3D:
                    {
                        var newOffset = GetPostion3D(array, 0, out Position3D position3D);
                        Object = position3D;
                        return newOffset;
                    }
                case TraCIConstants.POSITION_ROADMAP:
                    {
                        var newOffset = GetPositionRoadmap(array, 0, out RoadMapPosition roadMapPosition);
                        Object = roadMapPosition;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_BOUNDINGBOX:
                    {
                        var newOffset = GetBoundaryBox(array, 0, out BoundaryBox boundaryBox);
                        Object = boundaryBox;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_POLYGON:
                    {
                        var newOffset = GetPolygon(array, 0, out Polygon polygon);
                        Object = polygon;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_UBYTE:
                    {
                        var newOffset = GetUByte(array, 0, out TraCIUByte UByte);
                        Object = UByte.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_BYTE:
                    {
                        var newOffset = GetByte(array, 0, out TraCIByte sByte);
                        Object = sByte.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_INTEGER:
                    {
                        var newOffset = GetInteger(array, 0, out TraCIInteger integer);
                        Object = integer.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_FLOAT:
                    {
                        var newOffset = GetFloat(array, 0, out TraCIFloat Float);
                        Object = Float.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_DOUBLE:
                    {
                        var newOffset = GetDouble(array, 0, out TraCIDouble Double);
                        Object = Double.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_STRING:
                    {
                        var newOffset = GetString(array, 0, out TraCIString String);
                        Object = String.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_STRINGLIST:
                    {
                        var newOffset = GetStringList(array, 0, out TraCIStringList StringList);
                        Object = StringList.Value;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_COLOR:
                    {
                        var newOffset = GetColor(array, 0, out Color color);
                        Object = color;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_TLPHASELIST:
                    {
                        var newOffset = GetTrafficLightPhaseList(array, 0, out TrafficLightPhaseList trafficLightPhaseList);
                        Object = trafficLightPhaseList;
                        return newOffset;
                    }
                case TraCIConstants.TYPE_COMPOUND:
                    {
                        var take = array.Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
                        var count = BitConverter.ToInt32(take, 0);
                        var ctlist = new List<ComposedTypeBase>();
                        int offset = TraCIConstants.INTEGER_SIZE;
                        if (count > 0)
                        {
                            for (var i = 0; i <= count; ++i)
                            {
                                var ctype = array.Skip(offset).First();
                                ++offset;
                                switch (ctype)
                                {
                                    case TraCIConstants.POSITION_LON_LAT:
                                        {
                                            offset = GetPositionLonLat(array, offset, out LonLatPosition lonLatPosition);
                                            ctlist.Add(lonLatPosition);
                                            break;
                                        }
                                    case TraCIConstants.POSITION_2D:
                                        {
                                            offset = Get2DPosition(array, offset, out Position2D position2D);
                                            ctlist.Add(position2D);
                                            break;
                                        }
                                    case TraCIConstants.POSITION_LON_LAT_ALT:
                                        {
                                            offset = GetPositionLonLatAlt(array, offset, out LonLatAltPosition lonLatAltPosition);
                                            ctlist.Add(lonLatAltPosition);
                                            break;
                                        }
                                    case TraCIConstants.POSITION_3D:
                                        {
                                            offset = GetPostion3D(array, offset, out Position3D position3D);
                                            ctlist.Add(position3D);
                                            break;
                                        }
                                    case TraCIConstants.POSITION_ROADMAP:
                                        {
                                            offset = GetPositionRoadmap(array, offset, out RoadMapPosition roadMapPosition);
                                            ctlist.Add(roadMapPosition);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_BOUNDINGBOX:
                                        {
                                            offset = GetBoundaryBox(array, offset, out BoundaryBox boundaryBox);
                                            ctlist.Add(boundaryBox);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_POLYGON:
                                        {
                                            offset = GetPolygon(array, offset, out Polygon polygon);
                                            ctlist.Add(polygon);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_UBYTE:
                                        {
                                            offset = GetUByte(array, offset, out TraCIUByte UByte);
                                            ctlist.Add(UByte);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_BYTE:
                                        {
                                            offset = GetByte(array, offset, out TraCIByte Byte);
                                            ctlist.Add(Byte);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_INTEGER:
                                        {
                                            offset = GetInteger(array, offset, out TraCIInteger integer);
                                            ctlist.Add(integer);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_FLOAT:
                                        {
                                            offset = GetFloat(array, offset, out TraCIFloat Float);
                                            ctlist.Add(Float);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_DOUBLE:
                                        {
                                            offset = GetDouble(array, offset, out TraCIDouble Double);
                                            ctlist.Add(Double);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_STRING:
                                        {
                                            offset = GetString(array, offset, out TraCIString String);
                                            ctlist.Add(String);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_TLPHASELIST:
                                        {
                                            var newOffset = GetTrafficLightPhaseList(array, offset, out TrafficLightPhaseList trafficLightPhaseList);
                                            Object = trafficLightPhaseList;
                                            return newOffset;
                                        }
                                    case TraCIConstants.TYPE_COLOR:
                                        {
                                            offset = GetColor(array, offset, out Color color);
                                            ctlist.Add(color);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_STRINGLIST:
                                        {
                                            offset = GetStringList(array, offset, out TraCIStringList StringList);
                                            ctlist.Add(StringList);
                                            break;
                                        }
                                    case TraCIConstants.TYPE_COMPOUND:
                                        {
                                            offset += GetValueFromTypeAndArray(TraCIConstants.TYPE_COMPOUND, array.Skip(offset).ToArray(), out object InnerObject);
                                            var tmp = InnerObject as CompoundObject;
                                            ctlist.Add(tmp);
                                            i += 1 + tmp.Value.Count; //nested compounds are not nice! 
                                            break;
                                        }
                                }
                            }
                        }
                        var compoundObject = new CompoundObject();
                        compoundObject.Value = ctlist;
                        Object = compoundObject;
                        return offset;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }
        }

        private static int GetTrafficLightPhaseList(IEnumerable<byte> array, int offset, out TrafficLightPhaseList trafficLightPhaseList)
        {
            trafficLightPhaseList = new TrafficLightPhaseList();

            offset = GetByte(array, offset, out TraCIByte count);

            for (int i = 0; i < count.Value; i++)
            {
                offset = GetString(array, offset, out TraCIString precRoad);
                offset = GetString(array, offset, out TraCIString succRoad);
                offset = GetByte(array, offset, out TraCIByte phase);

                trafficLightPhaseList.Phases.Add(new TrafficLightPhase()
                {
                    PrecRoad = precRoad.Value,
                    SuccRoad = succRoad.Value,
                    Phase = (PhaseState)phase.Value
                });
            }
            return offset;
        }

        private static int GetColor(IEnumerable<byte> array, int offset, out Color color)
        {
            color = new Color
            {
                R = array.Skip(offset++).First(),
                G = array.Skip(offset++).First(),
                B = array.Skip(offset++).First(),
                A = array.Skip(offset++).First()
            };

            return offset;
        }

        private static int GetBoundaryBox(IEnumerable<byte> array, int offset, out BoundaryBox boundaryBox)
        {
            var bb = new BoundaryBox();
            var take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            bb.LowerLeftX = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            bb.LowerLeftY = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            bb.UpperRightX = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            bb.UpperRightY = BitConverter.ToDouble(take, 0);
            boundaryBox = bb;
            return offset;
        }

        private static int GetPositionRoadmap(IEnumerable<byte> array, int offset, out RoadMapPosition roadMapPosition)
        {
            var rmp = new RoadMapPosition();
            var sb = new StringBuilder();
            var take = array.Skip(offset).Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
            offset += TraCIConstants.INTEGER_SIZE;
            var length = BitConverter.ToInt32(take, 0);
            for (var j = 0; j < length; ++j)
            {
                sb.Append((char)array.Skip(j + TraCIConstants.INTEGER_SIZE).First());
                ++offset;
            }
            rmp.RoadId = sb.ToString();
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            rmp.Pos = BitConverter.ToDouble(take, 0);
            rmp.LaneId = array.Skip(offset).First();
            roadMapPosition = rmp;
            return offset;
        }

        private static int GetPostion3D(IEnumerable<byte> array, int offset, out Position3D position3D)
        {
            var pos3d = new Position3D();
            var take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            pos3d.X = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            pos3d.Y = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            pos3d.Z = BitConverter.ToDouble(take, 0);
            position3D = pos3d;
            return offset;
        }

        private static int GetPositionLonLatAlt(IEnumerable<byte> array, int offset, out LonLatAltPosition lonLatAltPosition)
        {
            var lonlatalt = new LonLatAltPosition();
            var take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            lonlatalt.Longitude = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            lonlatalt.Latitude = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            lonlatalt.Altitude = BitConverter.ToDouble(take, 0);
            lonLatAltPosition = lonlatalt;
            return offset;
        }

        private static int Get2DPosition(IEnumerable<byte> array, int offset, out Position2D position2D)
        {
            var pos2d = new Position2D();
            var take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            pos2d.X = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            pos2d.Y = BitConverter.ToDouble(take, 0);
            position2D = pos2d;

            return offset;
        }

        private static int GetPositionLonLat(IEnumerable<byte> array, int offset, out LonLatPosition lonLatPosition)
        {
            byte[] take;
            var lonlat = new LonLatPosition();
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            lonlat.Longitude = BitConverter.ToDouble(take, 0);
            take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            offset += TraCIConstants.DOUBLE_SIZE;
            lonlat.Latitude = BitConverter.ToDouble(take, 0);
            lonLatPosition = lonlat;
            return offset;
        }

        private static int GetStringList(IEnumerable<byte> array, int offset, out TraCIStringList StringList)
        {
            StringList = new TraCIStringList();

            var sb = new StringBuilder();
            var take = array.Skip(offset).Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
            var count = BitConverter.ToInt32(take, 0);
            var list = new List<string>();
            offset += TraCIConstants.INTEGER_SIZE;
            for (var i1 = 0; i1 < count; ++i1)
            {
                sb.Clear();
                take = array.Skip(offset).Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
                offset += TraCIConstants.INTEGER_SIZE;
                var length = BitConverter.ToInt32(take, 0);
                for (var j = 0; j < length; ++j)
                {
                    sb.Append((char)array.Skip(offset).First());
                    ++offset;
                }
                list.Add(sb.ToString());
            }
            StringList.Value = list;
            return offset;
        }

        private static int GetString(IEnumerable<byte> array, int offset, out TraCIString String)
        {
            String = new TraCIString();
            var sb = new StringBuilder();
            var take = array.Skip(offset).Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
            var length = BitConverter.ToInt32(take, 0);
            offset += TraCIConstants.INTEGER_SIZE;
            for (var i = 0; i < length; ++i)
            {
                sb.Append((char)array.Skip(offset).First());
                offset++;
            }
            String.Value = sb.ToString();

            return offset;
        }

        private static int GetDouble(IEnumerable<byte> array, int offset, out TraCIDouble Double)
        {
            Double = new TraCIDouble();
            var take = array.Skip(offset).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
            Double.Value = BitConverter.ToDouble(take, 0);
            return offset + TraCIConstants.DOUBLE_SIZE;
        }

        private static int GetFloat(IEnumerable<byte> array, int offset, out TraCIFloat Float)
        {
            Float = new TraCIFloat();
            var take = array.Skip(offset).Take(TraCIConstants.FLOAT_SIZE).Reverse().ToArray();
            Float.Value = BitConverter.ToSingle(take, 0);
            return offset + TraCIConstants.FLOAT_SIZE;
        }

        private static int GetInteger(IEnumerable<byte> array, int offset, out TraCIInteger integer)
        {
            integer = new TraCIInteger();
            var take = array.Skip(offset).Take(TraCIConstants.INTEGER_SIZE).Reverse().ToArray();
            integer.Value = BitConverter.ToInt32(take, 0);
            return offset + TraCIConstants.INTEGER_SIZE;
        }

        private static int GetByte(IEnumerable<byte> array, int offset, out TraCIByte Byte)
        {
            Byte = new TraCIByte
            {
                Value = array.Skip(offset).First()
            };
            return offset + TraCIConstants.BYTE_SIZE;
        }

        private static int GetUByte(IEnumerable<byte> array, int offset, out TraCIUByte Byte)
        {
            Byte = new TraCIUByte
            {
                Value = array.Skip(offset).First()
            };
            return offset + TraCIConstants.UBYTE_SIZE;
        }

        private static int GetPolygon(IEnumerable<byte> array, int offset, out Polygon pol)
        {
            byte[] take;
            var length = array.Skip(offset).First();
            int skip = offset + 1; // first byte is length of data

            pol = new Polygon();

            for (var j = 1; j <= length; j++)
            {
                var p = new Position2D();
                take = array.Skip(skip).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
                skip += TraCIConstants.DOUBLE_SIZE;
                p.X = BitConverter.ToDouble(take, 0);
                take = array.Skip(skip).Take(TraCIConstants.DOUBLE_SIZE).Reverse().ToArray();
                skip += TraCIConstants.DOUBLE_SIZE;
                p.Y = BitConverter.ToDouble(take, 0);
                pol.Points.Add(p);
            }
            return skip;
        }

        internal static byte[] GetMessageBytes(TraCICommand command)
        {
            return GetMessagesBytes(new[] { command });
        }

        internal static byte[] GetMessagesBytes(IEnumerable<TraCICommand> commands)
        {
            var cmessages = new List<List<byte>>();
            foreach (var c in commands)
            {
                var cmessage = new List<byte>();
                if (c.Contents == null)
                {
                    cmessage.Add(2); // no contents: only length self and id => 2
                }
                else if ((c.Contents.Length + 2) <= 255)
                {
                    cmessage.Add((byte)(c.Contents.Length + 2));
                }
                else
                {
                    cmessage.Add(0);
                    cmessage.AddRange(BitConverter.GetBytes(c.Contents.Length + 6).Reverse());
                }
                cmessage.Add(c.Identifier);
                if (c.Contents != null)
                {
                    cmessage.AddRange(c.Contents);
                }
                cmessages.Add(cmessage);
            }
            var totlength = cmessages.Select(x => x.Count).Sum() + 4;
            var totmessage = new List<byte>();
            totmessage.AddRange(BitConverter.GetBytes(totlength).Reverse());
            cmessages.ForEach(x => totmessage.AddRange(x));
            //totmessage.AddRange(BitConverter.GetBytes('\n'));
            return totmessage.ToArray();
        }

        internal static TraCIResult[] HandleResponse(byte[] response)
        {
            try
            {
                var revLength = response.Take(4).Reverse().ToArray();
                var totlength = BitConverter.ToInt32(revLength, 0);
                var i = 4;
                var results = new List<TraCIResult>();
                while (i < totlength)
                {
                    var trresult = new TraCIResult();
                    var j = 0;
                    int len = response[i + j++];
                    trresult.Length = len - 2; // bytes lenght will be: msg - length - id
                    if (len == 0)
                    {
                        if (j + i + 3 < totlength)
                        {
                            revLength = new byte[4];
                            revLength[0] = response[i + j + 3];
                            revLength[1] = response[i + j + 2];
                            revLength[2] = response[i + j + 1];
                            revLength[3] = response[i + j + 0];
                            len = BitConverter.ToInt32(revLength, 0);
                            trresult.Length = len - 6; // bytes lenght will be: msg - length - int32len - id
                            j += 4;
                        }
                        else
                        {
                            break;
                        }
                    }
                    trresult.Identifier = response[i + j++];

                    if (trresult.Identifier == TraCIConstants.CMD_SIMSTEP && totlength == response.Count() && len + 8 != response.Count())
                    {
                        var revSubCount = response.Skip(i + len).Take(4).Reverse().ToArray();
                        var subCount = BitConverter.ToInt32(revSubCount, 0);
                        len += 5;
                        for (int n = 0; n < subCount; n++)
                        {
                            var offset = i + len;
                            var revSubResponseLength = response.Skip(offset).Take(4).Reverse().ToArray();
                            offset += 4;
                            var subResponseLength = BitConverter.ToInt32(revSubResponseLength, 0);
                            len += subResponseLength;

                            // For some reason when the subscription is context subscription
                            // we need one extra byte in the len than simply adding SubResponseLength
                            var identifier = response.Skip(offset).First();
                            var identifierHighPart = identifier >> 4;
                            if (identifierHighPart == 0x09)
                                len++;
                        }
                        trresult.Length = --len;
                    }

                    var cmd = new List<byte>();
                    while (j < len)
                    {
                        cmd.Add(response[i + j++]);
                    }
                    trresult.Response = cmd.ToArray();
                    i += j;
                    results.Add(trresult);

                }
                return results.ToArray();
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        #endregion // Static Methods
    }
}