﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using RestLib;
using ServiceStack.Text;
using BitsharesRpc;

namespace BitsharesRpc
{
    public class BitsharesWallet
    {
		string m_rpcUrl;
		string m_rpcUsername;
		string m_rpcPassword;

		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Paul, 26/11/2014. </remarks>
		///
		/// <param name="rpcUrl">	  	Bitshares RPC root url. </param>
		/// <param name="rpcUsername">	The RPC username. </param>
		/// <param name="rpcPassword">	The RPC password. </param>
		public BitsharesWallet(string rpcUrl, string rpcUsername, string rpcPassword)
		{
			m_rpcUrl = rpcUrl;
			m_rpcUsername = rpcUsername;
			m_rpcPassword = rpcPassword;

			// configure servicestack.text to be able to parse the bitshares rpc responses
			JsConfig<DateTime>.DeSerializeFn = BitsharesDatetimeExtensions.ParseDateTime;
			JsConfig<decimal>.DeSerializeFn = s => decimal.Parse(s, NumberStyles.Float);
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			JsConfig.IncludeTypeInfo = false;
			JsConfig.IncludePublicFields = true;
			JsConfig.IncludeNullValues = true;
		}

		/// <summary>	Makes raw syncronus request </summary>
		///
		/// <remarks>	Paul, 26/11/2014. </remarks>
		///
		/// <typeparam name="T">	Request packet </typeparam>
		/// <param name="request">	. </param>
		///
		/// <returns>	A BitsharesResponse&lt;T&gt; </returns>
		public BitsharesResponse<T> MakeRawRequestSync<T>(BitsharesRequest request)
		{
			return Rest.JsonApiCallSync<BitsharesResponse<T>>(	m_rpcUrl, JsonSerializer.SerializeToString(request), 
																m_rpcUsername, m_rpcPassword);
		}

		/// <summary>	Makes syncronus request </summary>
		///
		/// <remarks>	Paul, 26/11/2014. </remarks>
		///
		/// <typeparam name="T">	Request packet. </typeparam>
		/// <param name="request">	. </param>
		///
		/// <returns>	A T. </returns>
		public T MakeRequestSync<T>(BitsharesRequest request)
		{
			BitsharesResponse<T> response = MakeRawRequestSync<T>(request);
			return response.result;
		}

		/// <summary>	Make a syncronus API post </summary>
		///
		/// <remarks>	Paul, 27/11/2014. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		/// <param name="method">	The method. </param>
		/// <param name="args">  	A variable-length parameters list containing arguments. </param>
		///
		/// <returns>	A T. </returns>
		public T ApiPostSync<T>(BitsharesMethods method, params object[] args)
		{
			return MakeRequestSync<T>(new BitsharesRequest(method, args));
		}

		/// <summary>	get_info command </summary>
		///
		/// <remarks>	Paul, 27/11/2014. </remarks>
		///
		/// <returns>	The information. </returns>
		public GetInfoResponse GetInfo()
		{
			return ApiPostSync<GetInfoResponse>(BitsharesMethods.get_info);
		}

		/// <summary>	Wallet account transaction history. </summary>
		///
		/// <remarks>	Paul, 27/11/2014. </remarks>
		///
		/// <param name="accountName">	(Optional) name of the account. </param>
		/// <param name="assetSymbol">	(Optional) the asset symbol. </param>
		/// <param name="limit">	  	(Optional) the limit. </param>
		///
		/// <returns>	A List&lt;BitsharesTransaction&gt; </returns>
		public List<BitsharesTransaction> WalletAccountTransactionHistory(string accountName=null, string assetSymbol=null, int limit=int.MaxValue)
		{
			return	ApiPostSync<List<BitsharesTransaction>>
					(
						BitsharesMethods.wallet_account_transaction_history, 
						accountName, 
						assetSymbol, 
						limit
					);
		}
    }
}